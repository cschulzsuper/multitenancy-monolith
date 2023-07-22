namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

public static class TickerUserSecretStates
{
    public static readonly string[] All = new[]
{
    Invalid, Reset, Pending, Confirmed
};

    public const string Invalid = "invalid";
    public const string Reset = "reset";
    public const string Pending = "pending";
    public const string Confirmed = "confirmed";
}