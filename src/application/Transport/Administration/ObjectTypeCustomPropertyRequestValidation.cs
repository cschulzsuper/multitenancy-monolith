using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class ObjectTypeCustomPropertyRequestValidation
{
    internal static void EnsureUniqueName(string uniqueName)
        => UniqueNameValidator.Ensure(uniqueName);
}