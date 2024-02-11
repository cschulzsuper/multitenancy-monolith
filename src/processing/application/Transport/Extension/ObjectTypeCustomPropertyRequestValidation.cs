using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

internal static class ObjectTypeCustomPropertyRequestValidation
{
    public static void EnsureUniqueName(string uniqueName)
        => UniqueNameValidator.Ensure(uniqueName);
}