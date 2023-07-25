using ChristianSchulz.MultitenancyMonolith.Jobs;
using Cronos;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Server.Jobs;

public sealed class CronExpressionSchedule : IPlannedJobSchedule
{

    private readonly CronExpression _expression;

    private readonly string _expressionLiteral;

    public CronExpressionSchedule(string expression)
    {
        _expression = CronExpression.Parse(expression, CronFormat.Standard);
        _expressionLiteral = expression;
    }

    public string Expression => _expressionLiteral;

    public DateTime Next(DateTime @base)
    {
        var adjustedBase = @base;

        do
        {
            var nextOccurrence = _expression.GetNextOccurrence(adjustedBase);

            if (nextOccurrence != null)
            {
                return nextOccurrence.Value;
            }

            adjustedBase = adjustedBase.AddMinutes(1);

        } while (true);
    }
}