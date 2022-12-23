namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation;

public interface IValidationRule<T>
{
    string ValidationMessage { get; }

    bool Check(T value);
}
