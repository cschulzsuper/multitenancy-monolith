namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;

public static class ScheduleExpressionTypes
{
    public static readonly string[] All =
    [
        CronExpression
    ];

    public const string CronExpression = "cron-expression";
}