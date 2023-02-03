using System.Collections.Concurrent;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

internal sealed class RepositoryContext<TEntity>
{
    public ConcurrentDictionary<object, TEntity> Data { get; } = new ConcurrentDictionary<object, TEntity>();

    public Func<TEntity, object> SnowflakeFactory { get; set; } = _ => Guid.NewGuid();

    public Func<TEntity, object> SnowflakeProvider { get; set; } = _ => Guid.Empty;

    public Action<IEnumerable<TEntity>, TEntity> Ensurance { get; set; } = (_, _) => { };


    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

    public IDisposable AcquireLock()
    {
        _lock.Wait();

        return new DataLock(_lock);
    }

    public async Task<IDisposable> AcquireLockAsync()
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