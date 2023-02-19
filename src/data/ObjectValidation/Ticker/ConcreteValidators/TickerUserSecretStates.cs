namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

public static class TickerUserSecretStates
{
    public static readonly string[] All = new[]
{
        Invalid, Temporary, Pending, Confirmed
    };

    public const string Invalid = "invalid";
    public const string Temporary = "temporary";
    public const string Pending = "pending";
    public const string Confirmed = "confirmed";
}
