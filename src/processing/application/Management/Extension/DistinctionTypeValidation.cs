using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Extension.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

internal static class DistinctionTypeValidation
{
    private static readonly Validator<DistinctionType> _insertValidator;
    private static readonly Validator<DistinctionType> _updateValidator;

    static DistinctionTypeValidation()
    {
        _insertValidator = new Validator<DistinctionType>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.DisplayName, DisplayNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.ObjectType, ObjectTypeValidator.CreateRules());
        _insertValidator.AddRules(x => x.CustomProperties, validator =>
            validator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules("custom property unique name")));


        _updateValidator = new Validator<DistinctionType>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _updateValidator.AddRules(x => x.DisplayName, DisplayNameValidator.CreateRules());
        _updateValidator.AddRules(x => x.ObjectType, ObjectTypeValidator.CreateRules());
        _updateValidator.AddRules(x => x.CustomProperties, validator =>
            validator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules("custom property unique name")));
    }

    public static void EnsureInsertable(DistinctionType distinctionType)
        => _insertValidator.Ensure(distinctionType);

    public static void EnsureUpdatable(DistinctionType distinctionType)
        => _updateValidator.Ensure(distinctionType);

    public static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    public static void EnsureUniqueName(string uniqueName)
        => UniqueNameValidator.Ensure(uniqueName);
}