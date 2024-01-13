using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Schedule;

[ObjectAnnotation(
    UniqueName = "planned-job",
    DisplayName = "Planned Job",
    Area = "schedule",
    Collection = "planned-jobs")]
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
