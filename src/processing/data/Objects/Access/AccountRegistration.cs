﻿using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Access;

public sealed class AccountRegistration : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string AuthenticationIdentity { get; set; }

    public required string AccountGroup { get; set; }

    public required string AccountMember { get; set; }

    public required string MailAddress { get; set; }

    public required string ProcessState { get; set; }

    public required Guid ProcessToken { get; set; }
}