using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountMemberValidation
{
    private readonly static Validator<AccountMember> _insertValidator;
    private readonly static Validator<AccountMember> _updateValidator;

    private readonly static Validator<long> _accountMemberSnowflakeValidator;
    private readonly static Validator<string> _accountMemberUniqueNameValidator;

    static AccountMemberValidation()
    {
        _insertValidator = new Validator<AccountMember>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());
        _insertValidator.AddRules(x => x.AuthenticationIdentities, validator =>
            validator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules("authentication identity unique name")));

        _updateValidator = new Validator<AccountMember>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _updateValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());
        _updateValidator.AddRules(x => x.AuthenticationIdentities, validator =>
            validator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules("authentication identity unique name")));

        _accountMemberSnowflakeValidator = new Validator<long>();
        _accountMemberSnowflakeValidator.AddRules(x => x, SnowflakeValidator.CreateRules("account member"));

        _accountMemberUniqueNameValidator = new Validator<string>();
        _accountMemberUniqueNameValidator.AddRules(x => x, UniqueNameValidator.CreateRules("account member"));
    }

    public static void EnsureInsertable(AccountMember @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(AccountMember @object)
        => _updateValidator.Ensure(@object);

    public static void EnsureAccountMember(long accountMember)
        => _accountMemberSnowflakeValidator.Ensure(accountMember);

    public static void EnsureAccountMember(string accountMember)
        => _accountMemberUniqueNameValidator.Ensure(accountMember);

}