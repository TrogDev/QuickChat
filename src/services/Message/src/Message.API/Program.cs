using QuickChat.Message.API.Extensions;
using QuickChat.Message.API.Grpc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddGrpc();

WebApplication app = builder.Build();

app.MapGrpcService<MessageService>();
app.MapGrpcService<SystemMessageService>();

app.Run();
