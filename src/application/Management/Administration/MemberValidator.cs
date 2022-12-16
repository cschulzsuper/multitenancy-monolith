using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class MemberValidator
{
    private readonly static Validator<Member> _validator;

    static MemberValidator()
    {
        _validator = new Validator<Member>();
        _validator.AddRules(x => x.Snowflake, ZeroValueValidator<long>.CreateRules("snowflake"));
        _validator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
    }

    internal static void Ensure(Member member)
        => _validator.Ensure(member);

    internal static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    internal static void EnsureUniqueName(string uniqueName)
        => UniqueNameValidator.Ensure(uniqueName);

}
