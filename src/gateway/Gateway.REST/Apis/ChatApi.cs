using Microsoft.AspNetCore.Mvc;
using QuickChat.Gateway.REST.Services;

namespace QuickChat.Gateway.REST.Apis;

public static class ChatApi
{
    public static RouteGroupBuilder MapChatApiV1(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/chats")
            .RequireAuthorization()
            .DisableAntiforgery()
            .HasApiVersion(1.0);

        api.MapGet("", GetCurrentUserChats);
        api.MapGet("{code}", GetChatByCode);
        api.MapPost("", CreateChat);
        api.MapPost("{id:Guid}/participants", JoinChat);

        return api;
    }

    public static async Task<IResult> GetCurrentUserChats([FromServices] IChatService service)
    {
        return Results.Ok(await service.GetCurrentUserChats());
    }

    public static async Task<IResult> GetChatByCode(
        [FromRoute] string code,
        [FromServices] IChatService service
    )
    {
        return Results.Ok(await service.GetChatByCode(code));
    }

    public static async Task<IResult> CreateChat(
        [FromForm] string name,
        [FromServices] IChatService service
    )
    {
        return Results.Ok(await service.CreateChat(name));
    }

    public static async Task<IResult> JoinChat(
        [FromRoute] Guid id,
        [FromForm] string name,
        [FromServices] IChatService service
    )
    {
        return Results.Ok(await service.JoinChat(id, name));
    }
}
