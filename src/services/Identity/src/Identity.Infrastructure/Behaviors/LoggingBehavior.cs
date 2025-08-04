using MediatR;
using Microsoft.Extensions.Logging;
using QuickChat.EventBus.Extensions;

namespace QuickChat.Identity.Infrastructure.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) =>
        this.logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Handling command {CommandName} ({@Command})",
            request.GetGenericTypeName(),
            request
        );
        TResponse response = await next();
        logger.LogInformation(
            "Command {CommandName} handled - response: {@Response}",
            request.GetGenericTypeName(),
            response
        );

        return response;
    }
}
