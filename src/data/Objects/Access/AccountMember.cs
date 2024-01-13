using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Access;

[ObjectAnnotation(
    UniqueName = "account-member",
    DisplayName = "Account Member",
    Area = "access",
    Collection = "account-members")]
public sealed class AccountMember : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }

    public required string MailAddress { get; set; }

    public ICollection<AccountMemberAuthenticationIdentity> AuthenticationIdentities { get; init; }
        = new List<AccountMemberAuthenticationIdentity>();
}