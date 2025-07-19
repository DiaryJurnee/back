using System.Reflection;
using Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public static partial class Configure
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatorConfiguration>? configure = null)
    {
        var config = new MediatorConfiguration();
        configure?.Invoke(config);

        services.AddScoped<Mediator>();
        services.AddScoped<IMediator>(s => s.GetRequiredService<Mediator>());
        services.AddScoped<ISender>(s => s.GetRequiredService<Mediator>());
        services.AddScoped<ICacher>(s => s.GetRequiredService<Mediator>());

        services.AddMemoryCache();

        foreach (var assembly in config.Assemblies)
            RegisterHandlersFromAssembly(services, assembly, config.ServiceLifetime);

        return services;
    }

    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddMediator(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
    }

    private static void RegisterHandlersFromAssembly(IServiceCollection services, Assembly assembly, ServiceLifetime lifetime)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericType)
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .ToList();

            foreach (var handlerInterface in handlerInterfaces)
                services.Add(new ServiceDescriptor(handlerInterface, handlerType, lifetime));
        }
    }
}
