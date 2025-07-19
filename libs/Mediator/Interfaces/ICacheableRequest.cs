namespace Mediator.Interfaces;


public interface ICacheableRequest<TResponse> : IRequest<TResponse>
{
    string GetCacheKey();
    TimeSpan CacheExpiration { get; }
}