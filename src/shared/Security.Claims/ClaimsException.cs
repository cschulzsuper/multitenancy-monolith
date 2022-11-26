namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;

public class ClaimsException : Exception
{
    public ClaimsException()
        : base()
    {

    }

    public ClaimsException(string message)
        : base(message)
    {

    }

    public ClaimsException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}