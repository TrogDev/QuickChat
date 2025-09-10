using Microsoft.AspNetCore.Diagnostics;
using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Exceptions.Handlers;

public class BadHttpRequestExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not BadHttpRequestException badHttpRequestException)
        {
            return false;
        }

        httpContext.Response.StatusCode = badHttpRequestException.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ApiExceptionModel(
                httpContext.Response.StatusCode,
                nameof(BadHttpRequestException),
                "Bad request",
                badHttpRequestException.Message
            ),
            cancellationToken
        );

        return true;
    }
}
