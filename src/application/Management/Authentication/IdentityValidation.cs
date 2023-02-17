using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal static class IdentityValidation
{
    private readonly static Validator<Identity> _insertValidator;
    private readonly static Validator<Identity> _updateValidator;

    private readonly static Validator<string> _identityValidator;

    static IdentityValidation()
    {
        _insertValidator = new Validator<Identity>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.Secret, SecretValidator.CreateRules());
        _insertValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());

        _updateValidator = new Validator<Identity>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _updateValidator.AddRules(x => x.Secret, SecretValidator.CreateRules());
        _updateValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());

        _identityValidator = new Validator<string>();
        _identityValidator.AddRules(x => x, UniqueNameValidator.CreateRules("identity"));
    }

    internal static void EnsureInsertable(Identity identity)
        => _insertValidator.Ensure(identity);

    public static void EnsureUpdatable(Identity identity)
        => _updateValidator.Ensure(identity);

    internal static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    internal static void EnsureIdentity(string identity)
        => _identityValidator.Ensure(identity);

}