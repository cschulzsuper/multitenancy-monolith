namespace ChristianSchulz.MultitenancyMonolith.Caching;

public interface IByteCache
{
    byte[] Get(string key);
    void Set(string key, byte[] value);
}