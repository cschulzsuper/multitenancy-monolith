namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;

public static class AuthenticationMethods
{
    public static readonly string[] All = [ Anonymouse, Maintenance, Secret ];

    public const string Anonymouse = "anonymouse";

    public const string Maintenance = "maintenance";

    public const string Secret = "secret";
}