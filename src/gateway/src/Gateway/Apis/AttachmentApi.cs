using Microsoft.AspNetCore.Mvc;
using QuickChat.Gateway.Services;

namespace QuickChat.Gateway.Apis;

public static class AttachmentApi
{
    public static RouteGroupBuilder MapAttachmentApiV1(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/attachments")
            .RequireAuthorization()
            .DisableAntiforgery()
            .HasApiVersion(1.0);

        api.MapGet("", GetBulk);
        api.MapPost("", Upload);

        return api;
    }

    public static async Task<IResult> GetBulk(
        [FromQuery] Guid[] ids,
        [FromServices] IAttachmentService service
    )
    {
        return Results.Ok(await service.GetAttachments(ids));
    }

    public static async Task<IResult> Upload(
        HttpRequest request,
        [FromForm] Enums.AttachmentType type,
        [FromServices] IAttachmentService service
    )
    {
        IFormCollection form = await request.ReadFormAsync();
        IFormFile? file = form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return Results.BadRequest("No file uploaded");
        }

        return Results.Ok(await service.UploadAttachment(file, type));
    }
}
