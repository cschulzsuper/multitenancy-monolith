using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class DistinctionTypeCustomPropertyRequestValidation
{
    internal static void EnsureUniqueName(string uniqueName)
        => UniqueNameValidator.Ensure(uniqueName);
}