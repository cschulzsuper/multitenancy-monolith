using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal static class MemberValidation
{
    private readonly static Validator<Member> _insertValidator;
    private readonly static Validator<Member> _updateValidator;

    private readonly static Validator<string> _memberValidator;

    static MemberValidation()
    {
        _insertValidator = new Validator<Member>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());
        _insertValidator.AddRules(x => x.Identities, validator =>
            validator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules("member identity unique name")));

        _updateValidator = new Validator<Member>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _updateValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());
        _updateValidator.AddRules(x => x.Identities, validator =>
            validator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules("member identity unique name")));

        _memberValidator = new Validator<string>();
        _memberValidator.AddRules(x => x, UniqueNameValidator.CreateRules("member"));
    }

    public static void EnsureInsertable(Member @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(Member @object)
        => _updateValidator.Ensure(@object);

    public static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    public static void EnsureMember(string member)
        => _memberValidator.Ensure(member);

}