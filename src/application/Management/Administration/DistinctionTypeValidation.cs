﻿using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class DistinctionTypeValidation
{
    private readonly static Validator<DistinctionType> _insertValidator;
    private readonly static Validator<DistinctionType> _updateValidator;

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

    internal static void EnsureInsertable(DistinctionType distinctionType)
        => _insertValidator.Ensure(distinctionType);

    public static void EnsureUpdatable(DistinctionType distinctionType)
        => _updateValidator.Ensure(distinctionType);

    internal static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);

    internal static void EnsureUniqueName(string uniqueName)
        => UniqueNameValidator.Ensure(uniqueName);
}