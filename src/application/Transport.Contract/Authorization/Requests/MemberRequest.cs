using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization.Requests;

public class MemberRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }
}