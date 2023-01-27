using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal static class MembershipValidator
{
    private readonly static Validator<Membership> _insertValidator;

    static MembershipValidator()
    {
        _insertValidator = new Validator<Membership>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValueValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.Identity, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.IdentitySnowflake, SnowflakeValidator.CreateRules());
        _insertValidator.AddRules(x => x.Member, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.MemberSnowflake, SnowflakeValidator.CreateRules());
        _insertValidator.AddRules(x => x.Group, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.GroupSnowflake, SnowflakeValidator.CreateRules());
    }
    public static void EnsureInsertable(Membership membership)
        => _insertValidator.Ensure(membership);

    public static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);
}