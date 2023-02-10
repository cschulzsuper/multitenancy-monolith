using System.Reflection;
using System.Runtime.CompilerServices;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Metadata;

public static class ObjectAnnotations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ExtractObjectType<TEntity>()
    {
        var entityType = typeof(TEntity);

        var objectTypeDefintion = entityType.GetCustomAttribute<ObjectAnnotationAttribute>();

        if (objectTypeDefintion != null)
        {
            return objectTypeDefintion.UniqueName
                .Replace('-', ' ');
        }
        else
        {
            return entityType.Name
                .ToLower();
        }
    }
}
