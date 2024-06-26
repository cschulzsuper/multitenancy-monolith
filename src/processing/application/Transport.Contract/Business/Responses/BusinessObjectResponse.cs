﻿using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business.Responses;

public sealed class BusinessObjectResponse
{
    public required string UniqueName { get; init; }

    public IDictionary<string, object> CustomProperties { get; init; } = new Dictionary<string, object>();
}