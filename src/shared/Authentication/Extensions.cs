using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;

namespace QuickChat.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
    {
        AuthenticationOptions? options = builder
            .Configuration.GetSection("Identity")
            .Get<AuthenticationOptions>();

        if (options == null || options.Url == null || options.Audience == null)
        {
            // No options, so no authentication
            return builder.Services;
        }

        builder
            .Services.AddAuthentication()
            .AddJwtBearer(o =>
            {
                string identityUrl = options.Url;
                string audience = options.Audience;

                o.Authority = identityUrl;
                o.RequireHttpsMetadata = false;
                o.Audience = audience;
                o.TokenValidationParameters.ValidateIssuer = false;
                o.TokenValidationParameters.ValidateAudience = false;
            });

        builder.Services.AddAuthorization();

        return builder.Services;
    }
}
