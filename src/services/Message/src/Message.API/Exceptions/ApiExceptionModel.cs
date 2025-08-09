using System.Text.Json;
using Grpc.Core;

namespace QuickChat.Message.API.Exceptions;

public record ApiExceptionModel
{
    public required int Status { get; init; }
    public required string Error { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }

    public RpcException ToRpcException(StatusCode statusCode)
    {
        Metadata metadata = [];
        metadata.Add("ApiExceptionModel", JsonSerializer.Serialize(this));
        return new RpcException(new Status(statusCode, Title), metadata);
    }
}
