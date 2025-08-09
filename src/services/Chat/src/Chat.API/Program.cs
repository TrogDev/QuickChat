using QuickChat.Chat.API.Extensions;
using QuickChat.Chat.API.Grpc;
using QuickChat.Chat.API.Interceptors;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddGrpc(o =>
{
    o.Interceptors.Add<InternalServerExceptionHandlerInterceptor>();
    o.Interceptors.Add<ValidationExceptionHandlerInterceptor>();
});

WebApplication app = builder.Build();

app.MapGrpcService<ChatService>();

app.Run();
