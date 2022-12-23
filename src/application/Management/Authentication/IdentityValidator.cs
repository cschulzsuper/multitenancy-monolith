using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal static class IdentityValidator
{
    private readonly static Validator<Identity> _insertValidator;

    static IdentityValidator()
    {
        _insertValidator = new Validator<Identity>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValueValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.Secret, SecretValidator.CreateRules());
        _insertValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());
    }

    internal static void EnsureInsertable(Identity identity)
        => _insertValidator.Ensure(identity);

    internal static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    internal static void EnsureUniqueName(string uniqueName)
        => UniqueNameValidator.Ensure(uniqueName);

}
