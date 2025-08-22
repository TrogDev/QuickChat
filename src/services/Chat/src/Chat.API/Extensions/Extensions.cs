using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using QuickChat.Chat.Application.Behaviors;
using QuickChat.Chat.Application.Commands;
using QuickChat.Chat.Application.Queries;
using QuickChat.Chat.Application.Repositories;
using QuickChat.Chat.Application.Services;
using QuickChat.Chat.Application.Validators;
using QuickChat.Chat.Infrastructure;
using QuickChat.Chat.Infrastructure.Repositories;
using QuickChat.EFHelper;

namespace QuickChat.Chat.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ChatContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
        });
        builder.Services.AddMigration<ChatContext>();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.AddRabbitMqEventBus("EventBus");

        builder.Services.AddTransient<ICodeGenerator, CodeGenerator>();

        builder.Services.AddSingleton<IValidator<CreateChatCommand>, CreateChatCommandValidator>();
        builder.Services.AddSingleton<IValidator<JoinChatCommand>, JoinChatCommandValidator>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetUserChatsQuery).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        builder.AddLoggingInfrastructure();
    }

    private static void AddLoggingInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        builder.Logging.AddOpenTelemetry(o =>
        {
            o.IncludeFormattedMessage = true;
            o.IncludeScopes = true;
            o.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Chat"));
            o.AddConsoleExporter();
        });
        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Chat"))
                    .AddAspNetCoreInstrumentation()
                    .AddNpgsql()
                    .AddSource("MediatorSender")
                    .AddConsoleExporter();
            });
    }
}
