using QuickChat.Message.API.Extensions;
using QuickChat.Message.API.Grpc;
using QuickChat.Message.API.Interceptors;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddGrpc(o =>
{
    o.Interceptors.Add<InternalServerExceptionHandlerInterceptor>();
    o.Interceptors.Add<ValidationExceptionHandlerInterceptor>();
});

WebApplication app = builder.Build();

app.MapGrpcService<MessageService>();
app.MapGrpcService<SystemMessageService>();

app.Run();
