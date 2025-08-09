using Microsoft.AspNetCore.Diagnostics;
using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Exceptions.Handlers;

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
