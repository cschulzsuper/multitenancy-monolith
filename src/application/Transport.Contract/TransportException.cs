using System;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public class TransportException : Exception
{

    public TransportException()
    : base()
    {

    }

    public TransportException(FormattableString message)
        : base(message.ToString())
    {

    }

    public TransportException(FormattableString message, Exception innerException)
        : base(message.ToString(), innerException)
    {

    }
}