using System.Text;
using Application.Common.Settings.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Modules.Auth.Scheme;

public static class AuthenticationScheme
{
    public static IServiceCollection AddAuthenticationScheme(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSettings settings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;
        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = signinKey,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}