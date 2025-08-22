using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using QuickChat.Authentication;
using QuickChat.Gateway.REST.Exceptions.Handlers;
using QuickChat.Gateway.REST.IntegrationEvents.EventHandlers;
using QuickChat.Gateway.REST.IntegrationEvents.Events;
using QuickChat.Gateway.REST.Options;
using QuickChat.Gateway.REST.Services;
using StackExchange.Redis;

namespace QuickChat.Gateway.REST.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<ApiExceptionHandler>();
        builder.Services.AddExceptionHandler<RpcExceptionHandler>();
        builder.Services.AddExceptionHandler<BadHttpRequestExceptionHandler>();
        builder.Services.AddExceptionHandler<InternalServerExceptionHandler>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        builder.ConfigureServiceOptions<IdentityOptions>("Identity");
        builder.ConfigureServiceOptions<AttachmentOptions>("Attachment");
        builder.ConfigureServiceOptions<ChatOptions>("Chat");
        builder.ConfigureServiceOptions<MessageOptions>("Message");

        builder.Services.AddHttpClient<IServiceHttpClient, ServiceHttpClient>();
        builder.Services.AddGrpcClient<Chat.ChatClient>(o =>
        {
            o.Address = new Uri(
                builder
                    .Configuration.GetSection("Services")
                    .GetSection("Chat")
                    .GetValue<string>("Url")!
            );
        });
        builder.Services.AddGrpcClient<Message.MessageClient>(o =>
        {
            o.Address = new Uri(
                builder
                    .Configuration.GetSection("Services")
                    .GetSection("Message")
                    .GetValue<string>("Url")!
            );
        });
        builder.Services.AddGrpcClient<SystemMessage.SystemMessageClient>(o =>
        {
            o.Address = new Uri(
                builder
                    .Configuration.GetSection("Services")
                    .GetSection("Message")
                    .GetValue<string>("Url")!
            );
        });
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IAttachmentService, AttachmentService>();
        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<ISystemMessageService, SystemMessageService>();

        builder
            .Services.AddSignalR()
            .AddStackExchangeRedis(
                builder.Configuration.GetConnectionString("Redis")!,
                o =>
                {
                    o.Configuration.ChannelPrefix = RedisChannel.Literal("QuickChatGateway");
                }
            );

        builder.AddDefaultAuthentication();
        builder.AddRabbitMqEventBus("EventBus").AddEventBusSubscriptions();

        builder.AddLoggingInfrastructure();
    }

    private static void ConfigureServiceOptions<TOptions>(
        this IHostApplicationBuilder builder,
        string section
    )
        where TOptions : class
    {
        builder.Services.Configure<TOptions>(
            builder.Configuration.GetSection("Services").GetSection(section)
        );
    }

    private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
    {
        eventBus.AddSubscription<
            UserJoinedChatIntegrationEvent,
            UserJoinedChatIntegrationEventHandler
        >();
        eventBus.AddSubscription<
            MessageAddedIntegrationEvent,
            MessageAddedIntegrationEventHandler
        >();
        eventBus.AddSubscription<
            MessageEditedIntegrationEvent,
            MessageEditedIntegrationEventHandler
        >();
        eventBus.AddSubscription<
            MessageDeletedIntegrationEvent,
            MessageDeletedIntegrationEventHandler
        >();
        eventBus.AddSubscription<
            SystemMessageAddedIntegrationEvent,
            SystemMessageAddedIntegrationEventHandler
        >();
    }

    private static void AddLoggingInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        builder.Logging.AddOpenTelemetry(o =>
        {
            o.IncludeFormattedMessage = true;
            o.IncludeScopes = true;
            o.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Gateway.REST"));
            o.AddConsoleExporter();
        });
        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Gateway.REST"))
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter();
            });
        ;
    }
}
