using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Admission;

[ObjectAnnotation(
    UniqueName = "authentication-identity-authentication-method",
    DisplayName = "Authentication Identity Authentication Method",
    Area = "admission",
    Collection = "authentication-identity-authentication-methods")]
public sealed class AuthenticationIdentityAuthenticationMethod : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required long AuthenticationIdentity { get; set; }
    public required string ClientName { get; set; }
    public required string AuthenticationMethod { get; set; }
}