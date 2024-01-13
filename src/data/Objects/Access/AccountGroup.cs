using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Access;

[ObjectAnnotation(
    UniqueName = "account-group",
    DisplayName = "Account Group",
    Area = "access",
    Collection = "account-groups")]
public sealed class AccountGroup : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }
}