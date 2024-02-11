using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule.Requests;

public sealed class PlannedJobRequest
{
    [Display(Name = "expression")]
    [RequiredText]
    public required string Expression { get; init; }
}