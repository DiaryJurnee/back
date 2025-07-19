using Application;
using Infrastructure;

namespace API.Dependency;

public static class Injection
{
    public static IServiceCollection Inject(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddApplication(configuration);

        return services;
    }
}
