using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickChat.EventBus.Abstractions;
using QuickChat.EventBusRabbitMQ;
using RabbitMQ.Client;

namespace Microsoft.Extensions.Hosting;

public static class RabbitMqDependencyInjectionExtensions
{
    private const string SectionName = "EventBus";

    public static IEventBusBuilder AddRabbitMqEventBus(
        this IHostApplicationBuilder builder,
        string connectionName
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton(sp =>
        {
            return new ConnectionFactory()
            {
                Uri = new Uri(builder.Configuration.GetConnectionString(connectionName)!),
                AutomaticRecoveryEnabled = true
            };
        });

        builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection(SectionName));
        builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
        builder.Services.AddSingleton<IHostedService>(
            sp => (RabbitMQEventBus)sp.GetRequiredService<IEventBus>()
        );

        return new EventBusBuilder(builder.Services);
    }

    private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }
}
