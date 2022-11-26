namespace ChristianSchulz.MultitenancyMonolith.Caching;

public interface IByteCache
{
    byte[] Get(string key);

    bool Has(string key, byte[] value);

    void Set(string key, byte[] value);
}