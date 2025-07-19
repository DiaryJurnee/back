using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

public class MediatorConfiguration
{
    private readonly List<Assembly> _assemblies = [];

    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
    public IReadOnlyList<Assembly> Assemblies => _assemblies.AsReadOnly();

    public MediatorConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        if (!_assemblies.Contains(assembly))
            _assemblies.Add(assembly);

        return this;
    }

    public MediatorConfiguration RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
            RegisterServicesFromAssembly(assembly);

        return this;
    }

    public MediatorConfiguration RegisterServicesFromAssemblyContaining<T>() => RegisterServicesFromAssembly(typeof(T).Assembly);

    public MediatorConfiguration RegisterServicesFromAssemblyContaining(Type type) => RegisterServicesFromAssembly(type.Assembly);

    public MediatorConfiguration AsTransient()
    {
        ServiceLifetime = ServiceLifetime.Transient;

        return this;
    }

    public MediatorConfiguration AsScoped()
    {
        ServiceLifetime = ServiceLifetime.Scoped;

        return this;
    }

    public MediatorConfiguration AsSingleton()
    {
        ServiceLifetime = ServiceLifetime.Singleton;

        return this;
    }
}