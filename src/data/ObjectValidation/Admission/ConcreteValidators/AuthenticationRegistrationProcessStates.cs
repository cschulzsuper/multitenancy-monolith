namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

public static class AuthenticationRegistrationProcessStates
{
    public static readonly string[] All = new[]
    {
        New, Confirmed, Approved, Completed
    };

    public const string New = "new";
    public const string Confirmed = "confirmed";
    public const string Approved = "approved";
    public const string Completed = "completed";
}
