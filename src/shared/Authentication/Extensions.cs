using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;

namespace QuickChat.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
    {
        // {
        //   "Identity": {
        //     "Url": "http://identity",
        //     "Audience": "Message"
        //    }
        // }

        IConfigurationSection identitySection = builder.Configuration.GetSection("Identity");

        if (
            !identitySection.Exists()
            || identitySection["Url"] == null
            || identitySection["Audience"] == null
        )
        {
            // No identity section, so no authentication
            return builder.Services;
        }

        // Prevent from mapping "sub" claim to nameidentifier.
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        builder
            .Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                string identityUrl = identitySection["Url"]!;
                string audience = identitySection["Audience"]!;

                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = audience;
                options.TokenValidationParameters.ValidIssuers = [identityUrl];
                options.TokenValidationParameters.ValidateAudience = false;
            });

        builder.Services.AddAuthorization();

        return builder.Services;
    }
}
