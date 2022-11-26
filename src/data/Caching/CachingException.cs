namespace ChristianSchulz.MultitenancyMonolith.Caching;

public sealed class CachingException : Exception
{

    public CachingException()
        : base()
    {

    }

    public CachingException(string message)
        : base(message)
    {

    }

    public CachingException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}