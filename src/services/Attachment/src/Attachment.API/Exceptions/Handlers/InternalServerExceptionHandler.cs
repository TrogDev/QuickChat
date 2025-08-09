using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace QuickChat.Attachment.API.Exceptions.Handlers;

public class InternalServerExceptionHandler(ILogger<InternalServerExceptionHandler> logger)
    : IExceptionHandler
{
    private readonly ILogger<InternalServerExceptionHandler> logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "An unhandled error occurred");

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            new ApiExceptionModel()
            {
                Error = "InternalServerException",
                Title = "An unhandled error occurred"
            },
            cancellationToken
        );

        return true;
    }
}
