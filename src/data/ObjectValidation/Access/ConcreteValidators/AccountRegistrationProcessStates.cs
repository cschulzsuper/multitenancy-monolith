namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;

public static class AccountRegistrationProcessStates
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
