namespace QuickChat.Attachment.API.Exceptions;

public record ApiExceptionModel
{
    public required int Status { get; init; }
    public required string Error { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }

    public IResult ToResult()
    {
        return Results.Json(this, statusCode: Status);
    }
}
