﻿using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

internal static class BusinessObjectValidation
{
    private readonly static Validator<BusinessObject> _insertValidator;
    private readonly static Validator<BusinessObject> _updateValidator;

    static BusinessObjectValidation()
    {
        _insertValidator = new Validator<BusinessObject>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());

        _updateValidator = new Validator<BusinessObject>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
    }

    internal static void EnsureInsertable(BusinessObject businessObject)
        => _insertValidator.Ensure(businessObject);

    public static void EnsureUpdatable(BusinessObject businessObject)
        => _updateValidator.Ensure(businessObject);

    internal static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    internal static void EnsureUniqueName(string uniqueName)
        => UniqueNameValidator.Ensure(uniqueName);

}