using Mediator.Delegates;
using Mediator.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Mediator;

public class Mediator(IServiceProvider serviceProvider, IMemoryCache cache) : IMediator
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();

        string? cacheKey = null;

        if (request is ICacheableRequest<TResponse> cacheableRequest)
        {
            cacheKey = cacheableRequest.GetCacheKey();

            if (cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
                return cachedResponse!;
        }

        var responseType = GetResponseType(requestType) ?? throw new InvalidOperationException($"Failed to determine response type for {requestType.Name}");

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"Handler for {requestType.Name} not found.");

        Task<TResponse> handlerDelegate()
        {
            var handleMethod = handler.GetType().GetMethod("Handle") ?? throw new InvalidOperationException($"Handle method not found for {handler.GetType().Name}");
            try
            {
                return (Task<TResponse>)handleMethod.Invoke(handler, [request, cancellationToken])!;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var pipelines = serviceProvider.GetServices(pipelineType).Cast<object>().Reverse().ToList();

        var chainedDelegate = pipelines.Aggregate(
            (RequestHandlerDelegate<TResponse>)handlerDelegate,
            (next, pipeline) => () =>
            {
                var handleMethod = pipeline.GetType().GetMethod("Handle")!;
                return (Task<TResponse>)handleMethod.Invoke(pipeline, [request, next, cancellationToken])!;
            });

        var result = await chainedDelegate();

        if (request is ICacheableRequest<TResponse> cacheableReq && cacheKey != null)
            cache.Set(cacheKey, result, cacheableReq.CacheExpiration);

        return result;
    }

    public void InvalidateCache(string cacheKey)
    {
        ArgumentNullException.ThrowIfNull(cacheKey);
        cache.Remove(cacheKey);
    }

    private static Type? GetResponseType(Type requestType)
    {
        var requestInterface = requestType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

        return requestInterface?.GetGenericArguments()[0];
    }
}

