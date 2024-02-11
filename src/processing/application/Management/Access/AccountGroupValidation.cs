using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountGroupValidation
{
    private static readonly Validator<AccountGroup> _insertValidator;
    private static readonly Validator<AccountGroup> _updateValidator;

    private static readonly Validator<long> _accountGroupSnowflakeValidator;
    private static readonly Validator<string> _accountGroupUniqueNameValidator;

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

    public static void EnsureInsertable(AccountGroup @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(AccountGroup @object)
        => _updateValidator.Ensure(@object);

    public static void EnsureAccountGroup(long accountGroup)
        => _accountGroupSnowflakeValidator.Ensure(accountGroup);

    public static void EnsureAccountGroup(string accountGroup)
        => _accountGroupUniqueNameValidator.Ensure(accountGroup);

}