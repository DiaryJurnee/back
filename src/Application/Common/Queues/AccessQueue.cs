using System.Collections.Concurrent;

namespace Application.Common.Queues;

public class AccessQueue
{
    private readonly ConcurrentDictionary<string, (SemaphoreSlim Semaphore, int Usage)> _locks = new();

    public async Task ExecuteAsync(string key, Func<Task> action, CancellationToken cancellationToken = default)
    {
        var sem = _locks.GetOrAdd(key, _ => (new SemaphoreSlim(1, 1), 0));
        Interlocked.Increment(ref sem.Usage);

        await sem.Semaphore.WaitAsync(cancellationToken);

        try
        {
            await action();
        }
        finally
        {
            sem.Semaphore.Release();

            if (Interlocked.Decrement(ref sem.Usage) == 0)
            {
                _locks.TryRemove(key, out _);
                sem.Semaphore.Dispose();
            }
        }
    }

    public async Task<T> ExecuteAsync<T>(string key, Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        var sem = _locks.GetOrAdd(key, _ => (new SemaphoreSlim(1, 1), 0));
        Interlocked.Increment(ref sem.Usage);

        await sem.Semaphore.WaitAsync(cancellationToken);

        try
        {
            return await action();
        }
        finally
        {
            sem.Semaphore.Release();

            if (Interlocked.Decrement(ref sem.Usage) == 0)
            {
                _locks.TryRemove(key, out _);
                sem.Semaphore.Dispose();
            }
        }
    }
}
