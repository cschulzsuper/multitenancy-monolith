using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business.Requests;

public class BusinessObjectRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }
}