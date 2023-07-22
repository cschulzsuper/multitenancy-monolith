using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Schedule
{
    [ObjectAnnotation("job",
        DisplayName = "Job",
        Area = "schedule",
        Collection = "jobs")]
    public sealed class Job : ICloneable
    {
        public object Clone()
        {
            var shallowCopy = new Job
            {
                Snowflake = Snowflake,
                UniqueName = UniqueName,
                JobType = JobType,
                JobExpression = JobExpression
            };

            return shallowCopy;
        }

        public long Snowflake { get; set; }

        public required string UniqueName { get; set; }

        public required string JobType { get; set; }

        public required string JobExpression { get; set; }

    }
}
