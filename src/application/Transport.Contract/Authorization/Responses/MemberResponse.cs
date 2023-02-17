﻿namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization.Responses;

public class MemberResponse
{
    public required string UniqueName { get; init; }

    public required string MailAddress { get; init; }
}