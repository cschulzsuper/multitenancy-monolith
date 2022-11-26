namespace ChristianSchulz.MultitenancyMonolith.Caching;

public interface IByteCacheFactory
{
    IByteCache Create(string prefix);
}