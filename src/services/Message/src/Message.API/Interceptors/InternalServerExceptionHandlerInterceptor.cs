using System.Net;
using Grpc.Core;
using Grpc.Core.Interceptors;
using QuickChat.Message.API.Exceptions;

namespace QuickChat.Message.API.Interceptors;

public class InternalServerExceptionHandlerInterceptor(
    ILogger<InternalServerExceptionHandlerInterceptor> logger
) : Interceptor
{
    private readonly ILogger<InternalServerExceptionHandlerInterceptor> logger = logger;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception e)
        {
            if (e is RpcException)
            {
                throw;
            }

            logger.LogError(e, "An unhandled error occurred");
            ApiExceptionModel exception =
                new()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Error = "InternalServerException",
                    Title = "An unhandled error occurred"
                };
            throw exception.ToRpcException(StatusCode.Unknown);
        }
    }
}
