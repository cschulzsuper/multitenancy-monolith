using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Metadata;

public static class ObjectAnnotations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ExtractObjectType<TEntity>()
    {
        var entityType = typeof(TEntity);

        return ExtractObjectType(entityType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ExtractObjectType(Type entityType)
    {
        var objectTypeDefinition = entityType.GetCustomAttribute<ObjectAnnotationAttribute>();

        if (objectTypeDefinition != null)
        {
            return objectTypeDefinition.UniqueName
                .Replace('-', ' ');
        }
        else
        {
            return entityType.Name
                .ToLower();
        }
    }
}