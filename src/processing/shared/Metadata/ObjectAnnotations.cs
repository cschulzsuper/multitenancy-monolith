using Humanizer;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Metadata;

public static class ObjectAnnotations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string UniqueName(Type objectType)
    {
        var uniqueName = objectType.Name.Kebaberize();

        return uniqueName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DisplayName(Type objectType)
    {
        var displayName = objectType.Name.Humanize(LetterCasing.Title);

        return displayName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DisplayNameLowerCase<T>()
    {
        var objectType = typeof(T);

        return DisplayNameLowerCase(objectType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DisplayNameLowerCase(Type objectType)
    {
        var displayName = objectType.Name.Humanize(LetterCasing.LowerCase);

        return displayName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Area(Type objectType)
    {
        var area = objectType.Namespace?.Split('.').Last().Kebaberize()
            ?? throw new UnreachableException($"The entity type '{objectType.Name}' does not have a namespace.");

        return area;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Collection(Type objectType)
    {
        var collection = objectType.Name.Kebaberize().Pluralize();

        return collection;
    }
}