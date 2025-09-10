using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Exceptions.Handlers;

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
            new ApiExceptionModel(
                httpContext.Response.StatusCode,
                "InternalServerException",
                "An unhandled error occurred"
            ),
            cancellationToken
        );

        return true;
    }
}
