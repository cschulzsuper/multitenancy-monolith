using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Extension.ConcreteAnnotations;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension.Requests;

public sealed class ObjectTypeCustomPropertyRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }

    [DisplayName]
    public required string DisplayName { get; init; }

    [Display(Name = "property name")]
    [CustomPropertyName]
    public required string PropertyName { get; init; }

    [Display(Name = "property type")]
    [CustomPropertyType]
    public required string PropertyType { get; init; }
}