﻿namespace ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;

public class DistinctionTypeCustomPropertyResponse
{
    public required string UniqueName { get; init; }
    public required string DistinctionType { get; init; }
}