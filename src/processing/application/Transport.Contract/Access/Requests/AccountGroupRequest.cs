using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;

public sealed class AccountGroupRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }
}