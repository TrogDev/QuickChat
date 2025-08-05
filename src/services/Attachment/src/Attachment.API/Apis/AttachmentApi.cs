using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickChat.Attachment.Application.Commands;
using QuickChat.Attachment.Application.Exceptions;
using QuickChat.Attachment.Application.Queries;
using QuickChat.Attachment.Domain.Enums;

namespace QuickChat.Attachment.API.Apis;

public static class AttachmentApi
{
    public static RouteGroupBuilder MapAttachmentApiV1(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/attachments/")
            .RequireAuthorization()
            .HasApiVersion(1.0);

        api.MapGet("", GetBulk);
        api.MapPost("", Upload).DisableAntiforgery();

        return api;
    }

    public static async Task<IResult> GetBulk(
        [FromQuery] string[] ids,
        [FromServices] ISender mediator
    )
    {
        GetAttachmentsByIdQuery query = new(ids.Select(e => new Guid(e)));
        return Results.Ok(await mediator.Send(query));
    }

    public static async Task<IResult> Upload(
        HttpRequest request,
        [FromForm] AttachmentType type,
        [FromServices] ISender mediator
    )
    {
        IFormCollection form = await request.ReadFormAsync();
        IFormFile? file = form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return Results.BadRequest("No file uploaded");
        }

        using Stream stream = file.OpenReadStream();

        UploadAttachmentCommand command =
            new(file.FileName, type, stream, file.ContentType, file.Length);

        Domain.Entities.Attachment result;

        try
        {
            result = await mediator.Send(command);
        }
        catch (InvalidAttachmentTypeException)
        {
            return Results.BadRequest("Invalid attachament type for this file.");
        }

        return Results.Ok(result);
    }
}
