using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class AuthenticationRegistrationValidation
{
    private readonly static Validator<AuthenticationRegistration> _insertValidator;
    private readonly static Validator<AuthenticationRegistration> _updateValidator;

    private readonly static Validator<Guid> _authenticationRegistrationProcessTokenValidator;

    static AuthenticationRegistrationValidation()
    {
        _insertValidator = new Validator<AuthenticationRegistration>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.AuthenticationIdentity, UniqueNameValidator.CreateRules("authentication identity"));
        _insertValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules("mail address"));
        _insertValidator.AddRules(x => x.ProcessState, AuthenticationRegistrationProcessStateValidator.CreateRules("process state"));
        _insertValidator.AddRules(x => x.ProcessToken, TokenValidator.CreateRules("process token"));
        _insertValidator.AddRules(x => x.Secret, SecretValidator.CreateRules());

        _updateValidator = new Validator<AuthenticationRegistration>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.AuthenticationIdentity, UniqueNameValidator.CreateRules("authentication identity"));
        _updateValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules("mail address"));
        _updateValidator.AddRules(x => x.ProcessState, AuthenticationRegistrationProcessStateValidator.CreateRules("process state"));
        _updateValidator.AddRules(x => x.Secret, SecretValidator.CreateRules());

        _authenticationRegistrationProcessTokenValidator = new Validator<Guid>();
        _authenticationRegistrationProcessTokenValidator.AddRules(x => x, TokenValidator.CreateRules("process token"));
    }

    public static void EnsureInsertable(AuthenticationRegistration @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(AuthenticationRegistration @object)
        => _updateValidator.Ensure(@object);

    public static void EnsureProcessToken(Guid processToken)
        => _authenticationRegistrationProcessTokenValidator.Ensure(processToken);

}