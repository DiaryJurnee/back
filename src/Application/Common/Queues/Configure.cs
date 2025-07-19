using Application.Common.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common;

public static partial class Configure
{
    public static IServiceCollection AddQueues(this IServiceCollection services)
    {
        services.AddSingleton<AccessQueue>();

        return services;
    }
}
