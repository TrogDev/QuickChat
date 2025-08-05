using Asp.Versioning.Builder;
using QuickChat.Attachment.API.Apis;
using QuickChat.Attachment.API.Extensions;
using QuickChat.Authentication;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddDefaultAuthentication();
builder.AddApplicationServices();
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
});

WebApplication app = builder.Build();

IVersionedEndpointRouteBuilder attachment = app.NewVersionedApi("Attachment");
attachment.MapAttachmentApiV1();

app.Run();
