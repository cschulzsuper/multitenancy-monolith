﻿using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Administration;

[ObjectAnnotation("object-type-custom-property",
    DisplayName = "Object Type Custom Property",
    Area = "administration",
    Collection = "object-type-custom-properties")]
public sealed class ObjectTypeCustomProperty : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public required string UniqueName { get; set; }

    public required string DisplayName { get; set; }

    public required string PropertyName { get; set; }

    public required string PropertyType { get; set; }
}