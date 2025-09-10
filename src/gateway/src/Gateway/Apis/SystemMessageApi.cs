using Microsoft.AspNetCore.Mvc;
using QuickChat.Gateway.Models;
using QuickChat.Gateway.Services;

namespace QuickChat.Gateway.Apis;

public static class SystemMessageApi
{
    public static RouteGroupBuilder MapSystemMessageApiV1(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/chats/{chatId:Guid}/system-messages")
            .RequireAuthorization()
            .DisableAntiforgery()
            .HasApiVersion(1.0);

        api.MapGet("", GetChatSystemMessages);

        return api;
    }

    public static async Task<IResult> GetChatSystemMessages(
        [FromServices] ISystemMessageService service,
        [FromRoute] Guid chatId,
        [FromQuery] int limit = 50,
        [FromQuery] long? cursor = null
    )
    {
        return Results.Ok(await service.GetChatSystemMessages(chatId, limit, cursor));
    }
}
