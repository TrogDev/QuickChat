using Asp.Versioning.Builder;
using QuickChat.Attachment.API.Apis;
using QuickChat.Attachment.API.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
});

WebApplication app = builder.Build();

IVersionedEndpointRouteBuilder attachment = app.NewVersionedApi("Attachment");
attachment.MapAttachmentApiV1();

app.Run();
