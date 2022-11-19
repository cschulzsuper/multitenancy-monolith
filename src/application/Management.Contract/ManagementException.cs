using System;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public class ManagementException : Exception
{

    public ManagementException()
        : base()
    {

    }

    public ManagementException(string message)
        : base(message)
    {

    }

    public ManagementException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}