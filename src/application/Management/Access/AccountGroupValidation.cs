using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountGroupValidation
{
    private readonly static Validator<AccountGroup> _insertValidator;
    private readonly static Validator<AccountGroup> _updateValidator;

    private readonly static Validator<long> _accountGroupSnowflakeValidator;
    private readonly static Validator<string> _accountGroupUniqueNameValidator;

    static AccountGroupValidation()
    {
        _insertValidator = new Validator<AccountGroup>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());

        _updateValidator = new Validator<AccountGroup>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());

        _accountGroupSnowflakeValidator = new Validator<long>();
        _accountGroupSnowflakeValidator.AddRules(x => x, SnowflakeValidator.CreateRules("account group"));

        _accountGroupUniqueNameValidator = new Validator<string>();
        _accountGroupUniqueNameValidator.AddRules(x => x, UniqueNameValidator.CreateRules("account group"));
    }

    internal static void EnsureInsertable(AccountGroup @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(AccountGroup @object)
        => _updateValidator.Ensure(@object);

    internal static void EnsureSnowflake(long accountGroup)
        => _accountGroupSnowflakeValidator.Ensure(accountGroup);

    internal static void EnsureIdentity(string accountGroup)
        => _accountGroupUniqueNameValidator.Ensure(accountGroup);

}