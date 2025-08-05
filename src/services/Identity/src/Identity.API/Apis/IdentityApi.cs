using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickChat.Identity.Application.Commands;

namespace QuickChat.Identity.API.Apis;

public static class IdentityApi
{
    public static RouteGroupBuilder MapIdentityApiV1(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/").HasApiVersion(1.0);

        api.MapPost("/users", CreateAnonymousUser);

        return api;
    }

    public static async Task<IResult> CreateAnonymousUser([FromServices] ISender mediator)
    {
        return Results.Ok(await mediator.Send(new CreateAnonymousUserCommand()));
    }
}
