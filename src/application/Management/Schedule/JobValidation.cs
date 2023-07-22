using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidationRules;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

internal static class JobValidation
{
    private readonly static Validator<Job> _insertValidator;
    private readonly static Validator<Job> _updateValidator;

    private readonly static Validator<string> _jobValidator;

    static JobValidation()
    {
        _insertValidator = new Validator<Job>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.JobType, JobTypeValidator.CreateRules());
        _insertValidator.AddRules(x => x.JobExpression, RequiredValidator.CreateRules("job expression"));
        _insertValidator.AddRules(x => x, new ScheduleExpressionValidationRule<Job>("job expression", x => x.JobType, x => x.JobExpression));

        _updateValidator = new Validator<Job>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.UniqueName, UniqueNameValidator.CreateRules());
        _insertValidator.AddRules(x => x.JobType, JobTypeValidator.CreateRules());
        _insertValidator.AddRules(x => x.JobExpression, RequiredValidator.CreateRules("job expression"));
        _insertValidator.AddRules(x => x, new ScheduleExpressionValidationRule<Job>("job expression", x => x.JobType, x => x.JobExpression));

        _jobValidator = new Validator<string>();
        _jobValidator.AddRules(x => x, UniqueNameValidator.CreateRules("job"));
    }

    public static void EnsureInsertable(Job @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(Job @object)
        => _updateValidator.Ensure(@object);

    public static void EnsureJob(string job)
        => _jobValidator.Ensure(job);
}