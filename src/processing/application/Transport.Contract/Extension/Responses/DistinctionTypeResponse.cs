﻿namespace ChristianSchulz.MultitenancyMonolith.Application.Extension.Responses;

public sealed class DistinctionTypeResponse
{
    public required string UniqueName { get; init; }
    public required string DisplayName { get; init; }
    public required string ObjectType { get; init; }
}