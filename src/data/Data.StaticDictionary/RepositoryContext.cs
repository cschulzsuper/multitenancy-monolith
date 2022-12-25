using System.Collections.Concurrent;

namespace ChristianSchulz.MultitenancyMonolith.Data;

internal sealed class RepositoryContext<TEntity>
{
    public ConcurrentDictionary<object, TEntity> Data { get; } = new ConcurrentDictionary<object, TEntity>();

    public Func<TEntity, object> SnowflakeFactory { get; set; } = _ => Guid.NewGuid();

    public Func<TEntity, object> SnowflakeProvider { get; set; } = _ => Guid.Empty;

    private SemaphoreSlim _lock = new SemaphoreSlim(1);

    public IDisposable AquireLock()
    {
        _lock.Wait();

        return new DataLock(_lock);
    }

    public async Task<IDisposable> AquireLockAsync()
    {
        await _lock.WaitAsync();

        return new DataLock(_lock);
    }

    internal record DataLock(SemaphoreSlim @lock) : IDisposable
    {
        public void Dispose()
        {
            @lock.Release();
        }
    }
}
