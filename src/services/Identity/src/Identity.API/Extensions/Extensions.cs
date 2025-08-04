using QuickChat.Identity.Application.Commands;
using QuickChat.Identity.Application.Services;
using QuickChat.Identity.Infrastructure;
using QuickChat.Identity.Infrastructure.Behaviors;
using QuickChat.Identity.Infrastructure.Services;

namespace QuickChat.Identity.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Identity"));
        builder.Services.AddTransient<IHasher, Sha256Hasher>();
        builder.Services.AddTransient<ITokenService, JwtTokenService>();
        builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateAnonymousUserCommandHandler).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
    }
}
