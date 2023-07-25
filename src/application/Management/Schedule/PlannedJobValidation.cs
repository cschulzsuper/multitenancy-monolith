using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidationRules;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

internal static class PlannedJobValidation
{
    private readonly static Validator<PlannedJob> _insertValidator;
    private readonly static Validator<PlannedJob> _updateValidator;

    private readonly static Validator<string> _plannedJobUniqueNameValidator;
    private readonly static Validator<long> _plannedJobSnowflakeValidator;

    static PlannedJobValidation()
    {
        _insertValidator = new Validator<PlannedJob>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.ExpressionType, ScheduleExpressionTypeValidator.CreateRules("expression type"));
        _insertValidator.AddRules(x => x.Expression, RequiredValidator.CreateRules("expression"));
        _insertValidator.AddRules(x => x, new ScheduleExpressionValidationRule<PlannedJob>("expression", x => x.ExpressionType, x => x.Expression));

        _updateValidator = new Validator<PlannedJob>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _updateValidator.AddRules(x => x.ExpressionType, ScheduleExpressionTypeValidator.CreateRules("expression type"));
        _updateValidator.AddRules(x => x.Expression, RequiredValidator.CreateRules("expression"));
        _updateValidator.AddRules(x => x, new ScheduleExpressionValidationRule<PlannedJob>("expression", x => x.ExpressionType, x => x.Expression));

        _plannedJobUniqueNameValidator = new Validator<string>();
        _plannedJobUniqueNameValidator.AddRules(x => x, UniqueNameValidator.CreateRules("expression job"));

        _plannedJobSnowflakeValidator = new Validator<long>();
        _plannedJobSnowflakeValidator.AddRules(x => x, SnowflakeValidator.CreateRules("expression job"));

    }

    public static void EnsureInsertable(PlannedJob @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(PlannedJob @object)
        => _updateValidator.Ensure(@object);

    public static void EnsurePlannedJob(string job)
        => _plannedJobUniqueNameValidator.Ensure(job);

    public static void EnsureJob(long job)
        => _plannedJobSnowflakeValidator.Ensure(job);
}