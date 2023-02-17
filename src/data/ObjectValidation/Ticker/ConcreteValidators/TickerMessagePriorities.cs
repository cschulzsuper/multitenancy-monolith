namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

public static class TickerMessagePriorities
{
    public static readonly string[] All = new[]
    {
        Low, Default, High,Catastrophe
    };

    public const string Low = "low";
    public const string Default = "default";
    public const string High = "high";
    public const string Catastrophe = "catastrophe";
}
