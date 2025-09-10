using Asp.Versioning.Builder;
using QuickChat.Gateway.Apis;
using QuickChat.Gateway.Extensions;
using QuickChat.Gateway.Hubs;

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
