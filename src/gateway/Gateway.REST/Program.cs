using Asp.Versioning.Builder;
using QuickChat.Gateway.REST.Apis;
using QuickChat.Gateway.REST.Extensions;
using QuickChat.Gateway.REST.Hubs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
});

WebApplication app = builder.Build();
app.UseExceptionHandler(o => { });

IVersionedEndpointRouteBuilder gateway = app.NewVersionedApi("Gateway");
gateway.MapIdentityApiV1();
gateway.MapAttachmentApiV1();
gateway.MapChatApiV1();
gateway.MapMessageApiV1();
gateway.MapSystemMessageApiV1();

app.MapHub<ChatHub>("ws/chats");

app.Run();
