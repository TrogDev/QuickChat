using QuickChat.Authentication;
using QuickChat.Gateway.REST.Exceptions.Handlers;
using QuickChat.Gateway.REST.Options;
using QuickChat.Gateway.REST.Services;

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

        builder.AddDefaultAuthentication();
        builder.AddRabbitMqEventBus("EventBus").AddEventBusSubscriptions();
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

    private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus) { }
}
