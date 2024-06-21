namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation;

public interface IValidationRule
{

}

public interface IValidationRule<T> : IValidationRule
{
    string ValidationMessage { get; }

    bool Check(T value);
}