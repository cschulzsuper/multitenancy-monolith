using System;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Metadata;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ObjectAnnotationAttribute : Attribute
{
    public required string UniqueName { get; init; }

    public required string DisplayName { get; init; }

    public required string Area { get; init; }

    public required string Collection { get; init; }
}