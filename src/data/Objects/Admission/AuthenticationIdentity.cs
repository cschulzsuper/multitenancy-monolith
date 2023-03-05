using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Admission;

[ObjectAnnotation("authentication-identity",
    DisplayName = "Authentication Identity",
    Area = "admission",
    Collection = "authentication-identities")]
public sealed class AuthenticationIdentity : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }
    public required string MailAddress { get; set; }
    public required string Secret { get; set; }
}