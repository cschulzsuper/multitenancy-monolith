using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class MemberValidator
{
    private readonly static Validator<Member> _insertValidator;
    private readonly static Validator<Member> _updateValidator;

    static MemberValidator()
    {
        _insertValidator = new Validator<Member>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValueValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());

        _updateValidator = new Validator<Member>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
    }

    public static void EnsureInsertable(Member member)
        => _insertValidator.Ensure(member);

    public static void EnsureUpdatable(Member member)
        => _updateValidator.Ensure(member);

    public static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    public static void EnsureUniqueName(string uniqueName)
        => UniqueNameValidator.Ensure(uniqueName);

}
