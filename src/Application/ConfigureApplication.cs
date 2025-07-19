using System.Reflection;
using Application.Common;
using Application.Common.Behaviours;
using CSharpFunctionalExtensions;
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
        services.AddQueues();

        services.AddMediator(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()).AsSingleton());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped, includeInternalTypes: true);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }
}
