using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Admission;

public sealed class AuthenticationIdentityAuthenticationMethod : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required long AuthenticationIdentity { get; set; }
    public required string ClientName { get; set; }
    public required string AuthenticationMethod { get; set; }
}