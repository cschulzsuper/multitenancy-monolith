using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountRegistrationValidation
{
    private readonly static Validator<AccountRegistration> _insertValidator;
    private readonly static Validator<AccountRegistration> _updateValidator;

    private readonly static Validator<Guid> _accountRegistrationProcessTokenValidator;
    private readonly static Validator<long> _accountRegistrationSnowflakeValidator;
    private readonly static Validator<string> _accountRegistrationAccountGroupValidator;

    static AccountRegistrationValidation()
    {
        _insertValidator = new Validator<AccountRegistration>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.AuthenticationIdentity, UniqueNameValidator.CreateRules("authentication identity"));
        _insertValidator.AddRules(x => x.AccountGroup, UniqueNameValidator.CreateRules("account group"));
        _insertValidator.AddRules(x => x.AccountMember, UniqueNameValidator.CreateRules("account member"));
        _insertValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());
        _insertValidator.AddRules(x => x.ProcessState, AccountRegistrationProcessStateValidator.CreateRules("process state"));
        _insertValidator.AddRules(x => x.ProcessToken, TokenValidator.CreateRules("process token"));

        _updateValidator = new Validator<AccountRegistration>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.AuthenticationIdentity, UniqueNameValidator.CreateRules("authentication identity"));
        _updateValidator.AddRules(x => x.AccountGroup, UniqueNameValidator.CreateRules("account group"));
        _updateValidator.AddRules(x => x.AccountMember, UniqueNameValidator.CreateRules("account member"));
        _updateValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());
        _updateValidator.AddRules(x => x.ProcessState, AccountRegistrationProcessStateValidator.CreateRules("process state"));
        _updateValidator.AddRules(x => x.ProcessToken, TokenValidator.CreateRules("process token"));

        _accountRegistrationProcessTokenValidator = new Validator<Guid>();
        _accountRegistrationProcessTokenValidator.AddRules(x => x, TokenValidator.CreateRules("process token"));

        _accountRegistrationSnowflakeValidator = new Validator<long>();
        _accountRegistrationSnowflakeValidator.AddRules(x => x, SnowflakeValidator.CreateRules("account registration"));

        _accountRegistrationAccountGroupValidator = new Validator<string>();
        _accountRegistrationAccountGroupValidator.AddRules(x => x, UniqueNameValidator.CreateRules("account group"));
    }

    public static void EnsureInsertable(AccountRegistration @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(AccountRegistration @object)
        => _updateValidator.Ensure(@object);

    public static void EnsureProcessToken(Guid processToken)
        => _accountRegistrationProcessTokenValidator.Ensure(processToken);

    public static void EnsureAccountRegistration(long accountRegistration)
        => _accountRegistrationSnowflakeValidator.Ensure(accountRegistration);

    public static void EnsureAccountGroup(string accountGroup)
        => _accountRegistrationAccountGroupValidator.Ensure(accountGroup);

}