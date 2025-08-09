using System.Text.Json;
using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;
using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Exceptions.Handlers;

public class RpcExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not RpcException rpcException)
        {
            return false;
        }

        Metadata.Entry? entry = rpcException.Trailers.Get("ApiExceptionModel");

        if (entry == null)
        {
            return false;
        }

        ApiExceptionModel apiExceptionModel = JsonSerializer.Deserialize<ApiExceptionModel>(
            entry.Value
        )!;

        httpContext.Response.StatusCode = apiExceptionModel.Status;
        await httpContext.Response.WriteAsJsonAsync(apiExceptionModel, cancellationToken);

        return true;
    }
}
