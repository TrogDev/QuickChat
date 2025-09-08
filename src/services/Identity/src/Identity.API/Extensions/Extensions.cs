using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using QuickChat.Identity.Application.Behaviors;
using QuickChat.Identity.Application.Commands;
using QuickChat.Identity.Application.Services;
using QuickChat.Identity.Infrastructure;
using QuickChat.Identity.Infrastructure.Services;

namespace QuickChat.Identity.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Identity"));
        builder.Services.AddTransient<IHasher, Sha256Hasher>();
        builder.Services.AddTransient<ITokenService, JwtTokenService>();
        builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateAnonymousUserCommandHandler).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        builder.AddLoggingInfrastructure();
    }

    private static void AddLoggingInfrastructure(this IHostApplicationBuilder builder)
    {
        string otlpEndpoint = builder.Configuration.GetValue<string>("OpenTelemetry:OtlpEndpoint")!;
        string serviceName = "Identity";

        builder.Logging.AddOpenTelemetry(loggerOptions =>
        {
            loggerOptions.IncludeFormattedMessage = true;
            loggerOptions.IncludeScopes = true;
            loggerOptions.SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService(serviceName)
            );
            loggerOptions.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(otlpEndpoint);
            });
        });
        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddSource("MediatorSender")
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(otlpEndpoint);
                    });
            });
    }
}
