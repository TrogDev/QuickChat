using Microsoft.AspNetCore.Diagnostics;

namespace QuickChat.Gateway.Exceptions.Handlers;

public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not ApiException apiException)
        {
            return false;
        }

        httpContext.Response.StatusCode = apiException.Model.Status;
        await httpContext.Response.WriteAsJsonAsync(apiException.Model, cancellationToken);

        return true;
    }
}
