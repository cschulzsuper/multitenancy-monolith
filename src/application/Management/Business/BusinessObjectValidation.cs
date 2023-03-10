using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

internal static class BusinessObjectValidation
{
    private readonly static Validator<BusinessObject> _insertValidator;
    private readonly static Validator<BusinessObject> _updateValidator;

    private readonly static Validator<string> _businessObjectValidator;

    static BusinessObjectValidation()
    {
        _insertValidator = new Validator<BusinessObject>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());

        _updateValidator = new Validator<BusinessObject>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());

        _businessObjectValidator = new Validator<string>();
        _businessObjectValidator.AddRules(x => x, UniqueNameValidator.CreateRules("business object"));

    }

    public static void EnsureInsertable(BusinessObject businessObject)
        => _insertValidator.Ensure(businessObject);

    public static void EnsureUpdatable(BusinessObject businessObject)
        => _updateValidator.Ensure(businessObject);

    public static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    public static void EnsureBusinessObject(string businessObject)
        => _businessObjectValidator.Ensure(businessObject);

}