using System;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public sealed class ManagementException : Exception
{
    private ManagementException(string message) : base(message) { }

}