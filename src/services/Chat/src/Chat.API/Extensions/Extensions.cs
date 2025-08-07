using Microsoft.EntityFrameworkCore;
using QuickChat.Chat.Application.Behaviors;
using QuickChat.Chat.Application.Queries;
using QuickChat.Chat.Application.Repositories;
using QuickChat.Chat.Application.Services;
using QuickChat.Chat.Infrastructure;
using QuickChat.Chat.Infrastructure.Repositories;
using QuickChat.EFHelper;

namespace QuickChat.Chat.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ChatContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
        });
        builder.Services.AddMigration<ChatContext>();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.AddRabbitMqEventBus("EventBus");

        builder.Services.AddTransient<ICodeGenerator, CodeGenerator>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetUserChatsQuery).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });
    }
}
