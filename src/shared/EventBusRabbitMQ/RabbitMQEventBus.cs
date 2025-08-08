using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using QuickChat.EventBus.Abstractions;
using QuickChat.EventBus.Events;
using QuickChat.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace QuickChat.EventBusRabbitMQ;

public sealed class RabbitMQEventBus(
    ILogger<RabbitMQEventBus> logger,
    IServiceProvider serviceProvider,
    IOptions<EventBusOptions> options,
    IOptions<EventBusSubscriptionInfo> subscriptionOptions,
    ConnectionFactory connectionFactory
) : IEventBus, IDisposable, IHostedService
{
    private const string exchangeName = "quickchat_event_bus";

    private readonly ILogger<RabbitMQEventBus> logger = logger;
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly ResiliencePipeline pipeline = CreateResiliencePipeline(
        options.Value.RetryCount
    );
    private readonly ConnectionFactory connectionFactory = connectionFactory;
    private readonly ActivitySource activitySource = new("EventBusRabbitMQ");
    private readonly string queueName = options.Value.SubscriptionClientName;
    private readonly EventBusSubscriptionInfo subscriptionInfo = subscriptionOptions.Value;
    private IConnection? rabbitMQConnection;
    private IChannel? consumerChannel;

    public async Task PublishAsync(IntegrationEvent @event)
    {
        string routingKey = @event.GetType().Name;

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace(
                "Creating RabbitMQ channel to publish event: {EventId} ({EventName})",
                @event.Id,
                routingKey
            );
        }

        if (rabbitMQConnection == null)
        {
            throw new InvalidOperationException("RabbitMQ connection is not open");
        }

        using IChannel channel = await rabbitMQConnection.CreateChannelAsync();

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
        }

        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: "direct");

        byte[] body = SerializeMessage(@event);

        await pipeline.Execute(async () =>
        {
            using Activity? activity = activitySource.StartActivity(
                $"{routingKey} publish",
                ActivityKind.Client
            );

            // Depending on Sampling (and whether a listener is registered or not), the activity above may not be created.
            // If it is created, then propagate its context. If it is not created, the propagate the Current context, if any.

            ActivityContext contextToInject = default;

            if (activity != null)
            {
                contextToInject = activity.Context;
            }
            else if (Activity.Current != null)
            {
                contextToInject = Activity.Current.Context;
            }

            BasicProperties properties = new() { DeliveryMode = DeliveryModes.Persistent };

            SetActivityContext(activity, routingKey, "publish");

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);
            }

            try
            {
                await channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );
            }
            catch (Exception ex)
            {
                activity.SetExceptionTags(ex);

                throw;
            }
        });
    }

    private static void SetActivityContext(Activity? activity, string routingKey, string operation)
    {
        if (activity is not null)
        {
            // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
            activity.SetTag("messaging.system", "rabbitmq");
            activity.SetTag("messaging.destination_kind", "queue");
            activity.SetTag("messaging.operation", operation);
            activity.SetTag("messaging.destination.name", routingKey);
            activity.SetTag("messaging.rabbitmq.routing_key", routingKey);
        }
    }

    public void Dispose()
    {
        consumerChannel?.Dispose();
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        using Activity? activity = activitySource.StartActivity(
            $"{eventArgs.RoutingKey} receive",
            ActivityKind.Client
        );

        SetActivityContext(activity, eventArgs.RoutingKey, "receive");

        string eventName = eventArgs.RoutingKey;
        string message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            activity?.SetTag("message", message);
            await ProcessEvent(eventName, message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error Processing message \"{Message}\"", message);
            activity.SetExceptionTags(e);
            // TODO: Add DLX
        }

        await consumerChannel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);
        }

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        if (!subscriptionInfo.EventTypes.TryGetValue(eventName, out Type? eventType))
        {
            logger.LogWarning("Unable to resolve event type for event name {EventName}", eventName);
            return;
        }

        IntegrationEvent integrationEvent = DeserializeMessage(message, eventType);

        // Get all the handlers using the event type as the key
        foreach (
            IIntegrationEventHandler handler in scope.ServiceProvider.GetKeyedServices<IIntegrationEventHandler>(
                eventType
            )
        )
        {
            await handler.Handle(integrationEvent);
        }
    }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:RequiresUnreferencedCode",
        Justification = "The 'JsonSerializer.IsReflectionEnabledByDefault' feature switch, which is set to false by default for trimmed .NET apps, ensures the JsonSerializer doesn't use Reflection."
    )]
    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050:RequiresDynamicCode",
        Justification = "See above."
    )]
    private IntegrationEvent DeserializeMessage(string message, Type eventType)
    {
        return (
            JsonSerializer.Deserialize(message, eventType, subscriptionInfo.JsonSerializerOptions)
            as IntegrationEvent
        )!;
    }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:RequiresUnreferencedCode",
        Justification = "The 'JsonSerializer.IsReflectionEnabledByDefault' feature switch, which is set to false by default for trimmed .NET apps, ensures the JsonSerializer doesn't use Reflection."
    )]
    [UnconditionalSuppressMessage(
        "AOT",
        "IL3050:RequiresDynamicCode",
        Justification = "See above."
    )]
    private byte[] SerializeMessage(IntegrationEvent @event)
    {
        return JsonSerializer.SerializeToUtf8Bytes(
            @event,
            @event.GetType(),
            subscriptionInfo.JsonSerializerOptions
        );
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Messaging is async so we don't need to wait for it to complete. On top of this
        // the APIs are blocking, so we need to run this on a background thread.
        _ = Task.Factory.StartNew(
            async () =>
            {
                try
                {
                    logger.LogInformation("Starting RabbitMQ connection on a background thread");

                    rabbitMQConnection = await connectionFactory.CreateConnectionAsync();
                    if (!rabbitMQConnection.IsOpen)
                    {
                        return;
                    }

                    if (logger.IsEnabled(LogLevel.Trace))
                    {
                        logger.LogTrace("Creating RabbitMQ consumer channel");
                    }

                    consumerChannel = await rabbitMQConnection.CreateChannelAsync();

                    consumerChannel.CallbackExceptionAsync += (sender, ea) =>
                    {
                        logger.LogWarning(ea.Exception, "Error with RabbitMQ consumer channel");
                        return Task.CompletedTask;
                    };

                    await consumerChannel.ExchangeDeclareAsync(
                        exchange: exchangeName,
                        type: "direct"
                    );

                    await consumerChannel.QueueDeclareAsync(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    if (logger.IsEnabled(LogLevel.Trace))
                    {
                        logger.LogTrace("Starting RabbitMQ basic consume");
                    }

                    AsyncEventingBasicConsumer consumer = new(consumerChannel);

                    consumer.ReceivedAsync += OnMessageReceived;

                    await consumerChannel.BasicConsumeAsync(
                        queue: queueName,
                        autoAck: false,
                        consumer: consumer
                    );

                    foreach (KeyValuePair<string, Type> eventType in subscriptionInfo.EventTypes)
                    {
                        await consumerChannel.QueueBindAsync(
                            queue: queueName,
                            exchange: exchangeName,
                            routingKey: eventType.Key
                        );
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error starting RabbitMQ connection");
                }
            },
            TaskCreationOptions.LongRunning
        );
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static ResiliencePipeline CreateResiliencePipeline(int retryCount)
    {
        RetryStrategyOptions retryOptions =
            new()
            {
                ShouldHandle = new PredicateBuilder()
                    .Handle<BrokerUnreachableException>()
                    .Handle<SocketException>(),
                MaxRetryAttempts = retryCount,
                DelayGenerator = (context) =>
                    ValueTask.FromResult(GenerateDelay(context.AttemptNumber))
            };

        return new ResiliencePipelineBuilder().AddRetry(retryOptions).Build();

        static TimeSpan? GenerateDelay(int attempt)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, attempt));
        }
    }
}
