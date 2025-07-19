namespace Mediator.Interfaces;

public interface ICacher
{
    public void InvalidateCache(string cacheKey);
}
