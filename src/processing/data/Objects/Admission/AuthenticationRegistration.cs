using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Admission;

public sealed class AuthenticationRegistration : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string AuthenticationIdentity { get; set; }

    public required string MailAddress { get; set; }

    public required string ProcessState { get; set; }

    public required Guid ProcessToken { get; set; }

    public required string Secret { get; set; }
}