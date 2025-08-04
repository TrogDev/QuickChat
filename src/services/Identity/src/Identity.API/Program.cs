using Asp.Versioning.Builder;
using QuickChat.Identity.API.Apis;
using QuickChat.Identity.API.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
});

WebApplication app = builder.Build();

app.MapDiscoveryApi();

IVersionedEndpointRouteBuilder identity = app.NewVersionedApi("Identity");
identity.MapIdentityApiV1();

app.Run();
