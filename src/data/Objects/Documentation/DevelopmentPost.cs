using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Documentation
{
    public sealed class DevelopmentPost : ICloneable
    {
        public object Clone()
        {
            var shallowCopy = new DevelopmentPost
            {
                Snowflake = Snowflake,
                Index = Index,
                Title = Title,
                DateTime = DateTime,
                Text = Text,
                Link = Link,
                Tags = Tags
            };

            return shallowCopy;
        }

        public long Snowflake { get; set; }

        public required long Index { get; set; }

        public required string Title { get; set; }

        public required DateTime DateTime { get; set; }

        public required string Text { get; set; }

        public required string Link { get; set; }

        public required string[] Tags { get; set; }


    }
}
