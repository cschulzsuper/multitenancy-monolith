using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Access;

[ObjectAnnotation("account-registration",
    DisplayName = "Account Registration",
    Area = "access",
    Collection = "account-registrations")]
public class AccountRegistration : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required long AuthenticationIdentity { get; set; }

    public required string AccountGroup { get; set; }

    public required string AccountMember { get; set; }

    public required string AccountMemberMailAddress { get; set; }

    public required string ProcessState { get; set; }

    public required Guid ProcessToken { get; set; }
}
