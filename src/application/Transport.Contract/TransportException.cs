using System;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public sealed class TransportException : Exception
{
    public TransportException()
        : base()
    {

    }

    public TransportException(string message)
        : base(message)
    {

    }

    public TransportException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}