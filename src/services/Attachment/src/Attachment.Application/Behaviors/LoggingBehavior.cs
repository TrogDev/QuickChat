using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using QuickChat.Logging;

namespace QuickChat.Attachment.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger = logger;
    private readonly ActivitySource activitySource = new("MediatorSender");

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        using Activity? activity = activitySource.StartActivity(
            $"{request.GetGenericTypeName()} Handle"
        );

        logger.LogInformation(
            "Handling command {CommandName} ({@Command})",
            request.GetGenericTypeName(),
            request
        );

        TResponse response;
        try
        {
            response = await next();
        }
        catch (Exception e)
        {
            activity.SetExceptionTags(e);
            throw;
        }

        logger.LogInformation("Command {CommandName} handled", request.GetGenericTypeName());

        return response;
    }
}
