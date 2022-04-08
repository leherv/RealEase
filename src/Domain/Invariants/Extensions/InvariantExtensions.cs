using Domain.ApplicationErrors;
using Domain.Results;
using Domain.Results.Extensions;

namespace Domain.Invariants.Extensions;

public static class InvariantExtensions
{
    public static Invariant Positive(this Invariant invariant, int value, string parameterName)
    {
        Positive(value, parameterName).OnFailure(invariant.AddViolation);
        return invariant;
    }

    public static Invariant GreaterThanOrEqualTo(this Invariant invariant, int valueToCompareTo, int value,
        string parameterName)
    {
        GreaterThanOrEqualTo(value, valueToCompareTo, parameterName).OnFailure(invariant.AddViolation);
        return invariant;
    }

    public static Invariant NotNullOrWhiteSpace(this Invariant invariant, string value, string parameterName)
    {
        IsNotNullOrEmpty(value, parameterName).OnFailure(invariant.AddViolation);
        return invariant;
    }

    private static Result IsNotNullOrEmpty(string value, string parameterName)
    {
        return Result.SuccessIf(!string.IsNullOrEmpty(value),
            Errors.Validation.RuleViolationError($"Parameter {parameterName} must not be null or whitespace."));
    }

    private static Result Positive<T>(this T value, string parameterName) where T : unmanaged, IComparable
    {
        return GreaterThan(value, default, parameterName);
    }

    private static Result GreaterThan<T>(T value, T valueToCompareTo, string parameterName) where T : IComparable
    {
        return Result.SuccessIf(
            value.CompareTo(valueToCompareTo) > 0,
            Errors.Validation.RuleViolationError($"Parameter {parameterName} must not be less than {valueToCompareTo}.")
        );
    }

    private static Result GreaterThanOrEqualTo<T>(T value, T valueToCompareTo, string parameterName)
        where T : IComparable
    {
        return Result.SuccessIf(
            value.CompareTo(valueToCompareTo) >= 0,
            Errors.Validation.RuleViolationError($"Parameter {parameterName} must not be less than {valueToCompareTo}.")
        );
    }
}