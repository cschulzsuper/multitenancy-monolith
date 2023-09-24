using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class AuthenticationIdentityAuthenticationMethodValidation
{
    private static readonly Validator<long> _authenticationIdentityValidator;
    private static readonly Validator<string> _clientNameValidator;
    private static readonly Validator<string> _authenticationMethodValidator;

    static AuthenticationIdentityAuthenticationMethodValidation()
    {
        _authenticationIdentityValidator = new Validator<long>();
        _authenticationIdentityValidator.AddRules(x => x, SnowflakeValidator.CreateRules("authentication identity"));

        _clientNameValidator = new Validator<string>();
        _clientNameValidator.AddRules(x => x, UniqueNameValidator.CreateRules("client name"));

        _authenticationMethodValidator = new Validator<string>();
        _authenticationMethodValidator.AddRules(x => x, AuthenticationMethodValidator.CreateRules());
    }

    public static void EnsureAuthenticationIdentity(long authenticationIdentity)
        => _authenticationIdentityValidator.Ensure(authenticationIdentity);

    public static void EnsureClientName(string clientName)
        => _clientNameValidator.Ensure(clientName);

    public static void EnsureAuthenticationMethod(string authenticationMethod)
        => _authenticationMethodValidator.Ensure(authenticationMethod);
}