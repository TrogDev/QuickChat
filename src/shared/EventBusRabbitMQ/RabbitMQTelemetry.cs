using System.Diagnostics;
using OpenTelemetry.Context.Propagation;

namespace QuickChat.EventBusRabbitMQ;

public class RabbitMQTelemetry
{
    public const string ActivitySourceName = "EventBusRabbitMQ";

    public ActivitySource ActivitySource { get; } = new(ActivitySourceName);
    public TextMapPropagator Propagator { get; } = Propagators.DefaultTextMapPropagator;
}
