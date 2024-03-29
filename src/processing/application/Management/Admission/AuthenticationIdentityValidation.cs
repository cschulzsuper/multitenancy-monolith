﻿using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class AuthenticationIdentityValidation
{
    private static readonly Validator<AuthenticationIdentity> _insertValidator;
    private static readonly Validator<AuthenticationIdentity> _updateValidator;

    private static readonly Validator<long> _authenticationIdentitySnowflakeValidator;
    private static readonly Validator<string> _authenticationIdentityUniqueNameValidator;

    static AuthenticationIdentityValidation()
    {
        _insertValidator = new Validator<AuthenticationIdentity>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.Secret, SecretValidator.CreateRules());
        _insertValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());

        _updateValidator = new Validator<AuthenticationIdentity>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _updateValidator.AddRules(x => x.Secret, SecretValidator.CreateRules());
        _updateValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());

        _authenticationIdentitySnowflakeValidator = new Validator<long>();
        _authenticationIdentitySnowflakeValidator.AddRules(x => x, SnowflakeValidator.CreateRules("authentication identity"));

        _authenticationIdentityUniqueNameValidator = new Validator<string>();
        _authenticationIdentityUniqueNameValidator.AddRules(x => x, UniqueNameValidator.CreateRules("authentication identity"));
    }

    public static void EnsureInsertable(AuthenticationIdentity @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(AuthenticationIdentity @object)
        => _updateValidator.Ensure(@object);

    public static void EnsureAuthenticationIdentity(long authenticationIdentity)
        => _authenticationIdentitySnowflakeValidator.Ensure(authenticationIdentity);

    public static void EnsureAuthenticationIdentity(string authenticationIdentity)
        => _authenticationIdentityUniqueNameValidator.Ensure(authenticationIdentity);

}