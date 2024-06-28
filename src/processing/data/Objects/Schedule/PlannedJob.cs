using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Schedule;

public sealed class PlannedJob : ICloneable
{
    public object Clone()
    {
        var shallowCopy = new PlannedJob
        {
            Snowflake = Snowflake,
            UniqueName = UniqueName,
            ExpressionType = ExpressionType,
            Expression = Expression
        };

        return shallowCopy;
    }

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }

    public required string ExpressionType { get; set; }

    public required string Expression { get; set; }

}
