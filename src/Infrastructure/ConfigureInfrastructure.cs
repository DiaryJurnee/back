using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Clusters;
using Domain.DayContents;
using Domain.Days;
using Domain.SystemRoles;
using Domain.Users;
using Domain.UsersWorkspaces;
using Domain.Workspaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddRepositories();
    }

    public static readonly IEnumerable<Type> entityTypes = [
        typeof(Cluster), typeof(DayContent), typeof(Day), typeof(SystemRole),
        typeof(Workspace), typeof(User), typeof(UserWorkspace)
    ];

    private static void AddRepositories(this IServiceCollection services)
    {
        foreach (var entityType in entityTypes)
        {
            var repositoryType = typeof(BaseRepository<>).MakeGenericType(entityType);
            var baseQueryType = typeof(IBaseQuery<>).MakeGenericType(entityType);
            var baseRepositoryType = typeof(IBaseRepository<>).MakeGenericType(entityType);

            services.AddScoped(repositoryType);
            services.AddScoped(baseQueryType, provider => provider.GetRequiredService(repositoryType));
            services.AddScoped(baseRepositoryType, provider => provider.GetRequiredService(repositoryType));
        }
    }
}
