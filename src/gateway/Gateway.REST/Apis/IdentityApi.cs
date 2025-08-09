using Microsoft.AspNetCore.Mvc;
using QuickChat.Gateway.REST.Services;

namespace QuickChat.Gateway.REST.Apis;

public static class IdentityApi
{
    public static RouteGroupBuilder MapIdentityApiV1(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/").HasApiVersion(1.0);

        api.MapPost("/users", CreateAnonymousUser);

        return api;
    }

    public static async Task<IResult> CreateAnonymousUser([FromServices] IIdentityService service)
    {
        return Results.Ok(await service.CreateAnonymousUser());
    }
}
