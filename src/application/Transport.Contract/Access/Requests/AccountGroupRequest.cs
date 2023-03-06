using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;

public class AccountGroupRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }
}