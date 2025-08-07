using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Polly.Retry;
using QuickChat.Logging;

namespace QuickChat.EventBusRabbitMQ;

public sealed class RabbitMQEventBus(
    ILogger<RabbitMQEventBus> logger,
    IServiceProvider serviceProvider,
    IOptions<EventBusOptions> options,
    IOptions<EventBusSubscriptionInfo> subscriptionOptions,
    RabbitMQTelemetry rabbitMQTelemetry
) : IEventBus, IDisposable, IHostedService
{
    private const string ExchangeName = "quickchat_event_bus";

    private readonly ILogger<RabbitMQEventBus> logger = logger;
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly ResiliencePipeline _pipeline = CreateResiliencePipeline(
        options.Value.RetryCount
    );
    private readonly TextMapPropagator propagator = rabbitMQTelemetry.Propagator;
    private readonly ActivitySource activitySource = rabbitMQTelemetry.ActivitySource;
    private readonly string queueName = options.Value.SubscriptionClientName;
    private readonly EventBusSubscriptionInfo subscriptionInfo = subscriptionOptions.Value;
    private IConnection rabbitMQConnection;

    private IModel consumerChannel;

    public Task PublishAsync(IntegrationEvent @event)
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

        using IModel channel =
            rabbitMQConnection?.CreateModel()
            ?? throw new InvalidOperationException("RabbitMQ connection is not open");

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
        }

        channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");

        byte[] body = SerializeMessage(@event);

        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
        string activityName = $"{routingKey} publish";

        return _pipeline.Execute(() =>
        {
            using Activity activity = activitySource.StartActivity(
                activityName,
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

            IBasicProperties properties = channel.CreateBasicProperties();
            // persistent
            properties.DeliveryMode = 2;

            static void InjectTraceContextIntoBasicProperties(
                IBasicProperties props,
                string key,
                string value
            )
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            }

            propagator.Inject(
                new PropagationContext(contextToInject, Baggage.Current),
                properties,
                InjectTraceContextIntoBasicProperties
            );

            SetActivityContext(activity, routingKey, "publish");

            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);
            }

            try
            {
                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                activity.SetExceptionTags(ex);

                throw;
            }
        });
    }

    private static void SetActivityContext(Activity activity, string routingKey, string operation)
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
        static IEnumerable<string> ExtractTraceContextFromBasicProperties(
            IBasicProperties props,
            string key
        )
        {
            if (props.Headers.TryGetValue(key, out object value))
            {
                byte[] bytes = value as byte[];
                return [Encoding.UTF8.GetString(bytes)];
            }
            return [];
        }

        // Extract the PropagationContext of the upstream parent from the message headers.
        PropagationContext parentContext = propagator.Extract(
            default,
            eventArgs.BasicProperties,
            ExtractTraceContextFromBasicProperties
        );
        Baggage.Current = parentContext.Baggage;

        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md
        string activityName = $"{eventArgs.RoutingKey} receive";

        using Activity activity = activitySource.StartActivity(
            activityName,
            ActivityKind.Client,
            parentContext.ActivityContext
        );

        SetActivityContext(activity, eventArgs.RoutingKey, "receive");

        string eventName = eventArgs.RoutingKey;
        string message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            activity?.SetTag("message", message);

            if (
                message.Contains(
                    "throw-fake-exception",
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
            {
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
            }

            await ProcessEvent(eventName, message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error Processing message \"{Message}\"", message);
            activity.SetExceptionTags(e);
            // TODO: Add DLX
        }

        consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);
        }

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        if (!subscriptionInfo.EventTypes.TryGetValue(eventName, out Type eventType))
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
        return JsonSerializer.Deserialize(
                message,
                eventType,
                subscriptionInfo.JsonSerializerOptions
            ) as IntegrationEvent;
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
            () =>
            {
                try
                {
                    logger.LogInformation("Starting RabbitMQ connection on a background thread");

                    rabbitMQConnection = serviceProvider.GetRequiredService<IConnection>();
                    if (!rabbitMQConnection.IsOpen)
                    {
                        return;
                    }

                    if (logger.IsEnabled(LogLevel.Trace))
                    {
                        logger.LogTrace("Creating RabbitMQ consumer channel");
                    }

                    consumerChannel = rabbitMQConnection.CreateModel();

                    consumerChannel.CallbackException += (sender, ea) =>
                    {
                        logger.LogWarning(ea.Exception, "Error with RabbitMQ consumer channel");
                    };

                    consumerChannel.ExchangeDeclare(exchange: ExchangeName, type: "direct");

                    consumerChannel.QueueDeclare(
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

                    consumer.Received += OnMessageReceived;

                    consumerChannel.BasicConsume(
                        queue: queueName,
                        autoAck: false,
                        consumer: consumer
                    );

                    foreach (KeyValuePair<string, Type> eventType in subscriptionInfo.EventTypes)
                    {
                        consumerChannel.QueueBind(
                            queue: queueName,
                            exchange: ExchangeName,
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
        // See https://www.pollydocs.org/strategies/retry.html
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
