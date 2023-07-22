using System;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Metadata;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ObjectAnnotationAttribute : Attribute
{
    private string? _displayName;

    private string? _area;

    private string? _collection;

    public ObjectAnnotationAttribute(string uniqueName)
    {
        UniqueName = uniqueName;
    }

    public string UniqueName { get; }

    public string DisplayName
    {
        get => _displayName ?? throw new MemberAccessException("No value for 'DisplayName' is set.");

        init
        {
            _displayName = value;
        }
    }

    public string Area
    {
        get => _area ?? throw new MemberAccessException("No value for 'Area' is set.");
        init
        {
            _area = value;
        }
    }

    public string Collection
    {
        get => _collection ?? throw new MemberAccessException("No value for 'Collection' is set.");
        init
        {
            _collection = value;
        }
    }
}