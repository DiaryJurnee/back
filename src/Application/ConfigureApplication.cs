using System.Reflection;
using Application.Common;
using Application.Common.Behaviours;
using Application.Common.Interfaces.Services;
using Application.Common.Services;
using Application.Common.Settings.GoogleAuth;
using Application.Common.Settings.Jwt;
using FluentValidation;
using Mediator;
using Mediator.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSettings(configuration);

        services.AddHttpClient();

        services.AddQueues();

        services.AddMediator(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()).AsScoped());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped, includeInternalTypes: true);

        services.AddServices();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IGoogleTokenService, GoogleTokenService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }

    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoogleAuthSettings>(
            configuration.GetSection("Authorization:Google")
        );

        services.Configure<JwtSettings>(
            configuration.GetSection(nameof(JwtSettings))
        );

        return services;
    }
}
