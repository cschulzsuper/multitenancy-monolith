namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;

public static class AuthenticationMethods
{
    public static readonly string[] All = new[]
    {
        Anonymouse, 
        Secret
    };

    public const string Anonymouse = "anonymouse";

    public const string Secret = "secret";
}