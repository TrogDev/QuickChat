using QuickChat.Chat.API.Extensions;
using QuickChat.Chat.API.Grpc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddGrpc();

WebApplication app = builder.Build();

app.MapGrpcService<ChatService>();

app.Run();
