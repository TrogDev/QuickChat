using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using QuickChat.EFHelper;
using QuickChat.Message.Application.Behaviors;
using QuickChat.Message.Application.Commands;
using QuickChat.Message.Application.IntegrationEvents.EventHandlers;
using QuickChat.Message.Application.IntegrationEvents.Events;
using QuickChat.Message.Application.Repositories;
using QuickChat.Message.Application.Services;
using QuickChat.Message.Application.Validators;
using QuickChat.Message.Infrastructure;
using QuickChat.Message.Infrastructure.Repositories;
using QuickChat.Message.Infrastructure.Services;

namespace QuickChat.Message.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<MessageContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
        });
        builder.Services.AddMigration<MessageContext>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<ISystemMessageRepository, SystemMessageRepository>();
        builder.AddRabbitMqEventBus("EventBus").AddEventBusSubscriptions();

        builder.Services.Configure<AttachmentOptions>(
            builder.Configuration.GetSection("Attachment")
        );
        builder.Services.AddHttpClient<IAttachmentService, AttachmentService>();

        builder.Services.AddSingleton<IValidator<AddMessageCommand>, AddMessageCommandValidator>();
        builder.Services.AddSingleton<
            IValidator<EditMessageCommand>,
            EditMessageCommandValidator
        >();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AddMessageCommand).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        builder.AddLoggingInfrastructure();
    }

    private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
    {
        eventBus.AddSubscription<
            UserJoinedChatIntegrationEvent,
            UserJoinedChatIntegrationEventHandler
        >();
    }

    private static void AddLoggingInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        builder.Logging.AddOpenTelemetry(o =>
        {
            o.IncludeFormattedMessage = true;
            o.IncludeScopes = true;
            o.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Message"));
            o.AddConsoleExporter();
        });
        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Message"))
                    .AddAspNetCoreInstrumentation()
                    .AddNpgsql()
                    .AddHttpClientInstrumentation()
                    .AddSource("MediatorSender")
                    .AddConsoleExporter();
            });
    }
}
