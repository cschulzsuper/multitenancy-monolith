using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class ObjectTypeValidation
{
    private readonly static Validator<ObjectType> _insertValidator;
    private readonly static Validator<ObjectType> _updateValidator;

    private readonly static Validator<string> _uniqueNameValidator;

    static ObjectTypeValidation()
    {
        _uniqueNameValidator = new Validator<string>();
        _uniqueNameValidator.AddRules(x => x, ObjectTypeValidator.CreateRules("unique name"));

        _insertValidator = new Validator<ObjectType>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, ObjectTypeValidator.CreateRules("unique name"));
        _insertValidator.AddRules(x => x.CustomProperties, validator =>
        {
            validator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules("custom property unique name"));
            validator.AddRules(x => x.DisplayName, DisplayNameValidator.CreateRules());
            validator.AddRules(x => x.PropertyName, CustomPropertyNameValidator.CreateRules("property name"));
            validator.AddRules(x => x.PropertyType, CustomPropertyTypeValidator.CreateRules("property type"));
        });


        _updateValidator = new Validator<ObjectType>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, ObjectTypeValidator.CreateRules("unique name"));
        _updateValidator.AddRules(x => x.CustomProperties, validator =>
        {
            validator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules("custom property unique name"));
            validator.AddRules(x => x.DisplayName, DisplayNameValidator.CreateRules());
            validator.AddRules(x => x.PropertyName, CustomPropertyNameValidator.CreateRules("property name"));
            validator.AddRules(x => x.PropertyType, CustomPropertyTypeValidator.CreateRules("property type"));
        });
    }

    public static void EnsureInsertable(ObjectType objectType)
        => _insertValidator.Ensure(objectType);

    public static void EnsureUpdatable(ObjectType objectType)
        => _updateValidator.Ensure(objectType);

    public static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    public static void EnsureUniqueName(string uniqueName)
        => _uniqueNameValidator.Ensure(uniqueName);
}