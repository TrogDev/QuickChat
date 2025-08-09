using Microsoft.AspNetCore.Mvc;
using QuickChat.Gateway.REST.Models;
using QuickChat.Gateway.REST.Services;

namespace QuickChat.Gateway.REST.Apis;

public static class MessageApi
{
    public static RouteGroupBuilder MapMessageApiV1(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/chats/{chatId:Guid}/messages")
            .RequireAuthorization()
            .DisableAntiforgery()
            .HasApiVersion(1.0);

        api.MapGet("", GetChatMessages);
        api.MapPost("", AddMessage);
        api.MapPut("{id:long}", EditMessage);
        api.MapDelete("{id:long}", DeleteMessage);

        return api;
    }

    public static async Task<IResult> GetChatMessages(
        [FromServices] IMessageService service,
        [FromRoute] Guid chatId,
        [FromQuery] int limit = 50,
        [FromQuery] long? cursor = null
    )
    {
        return Results.Ok(await service.GetChatMessages(chatId, limit, cursor));
    }

    public static async Task<IResult> AddMessage(
        [FromServices] IMessageService service,
        [FromRoute] Guid chatId,
        [FromBody] UpdateMessageModel model
    )
    {
        await service.AddMessage(chatId, model);
        return Results.NoContent();
    }

    public static async Task<IResult> EditMessage(
        [FromServices] IMessageService service,
        [FromRoute] Guid chatId,
        [FromRoute] long id,
        [FromBody] UpdateMessageModel model
    )
    {
        await service.EditMessage(chatId, id, model);
        return Results.NoContent();
    }

    public static async Task<IResult> DeleteMessage(
        [FromServices] IMessageService service,
        [FromRoute] Guid chatId,
        [FromRoute] long id
    )
    {
        await service.DeleteMessage(chatId, id);
        return Results.NoContent();
    }
}
