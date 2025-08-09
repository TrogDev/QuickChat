using System.Net;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using QuickChat.Message.API.Exceptions;

namespace QuickChat.Message.API.Interceptors;

public class ValidationExceptionHandlerInterceptor : Interceptor
{
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
        catch (ValidationException)
        {
            ApiExceptionModel exception =
                new()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Error = nameof(ValidationException),
                    Title = "One or more validation errors occurred"
                };
            throw exception.ToRpcException(StatusCode.InvalidArgument);
        }
    }
}
