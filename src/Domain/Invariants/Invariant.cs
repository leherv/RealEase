using Domain.ApplicationErrors;
using Domain.Results;

namespace Domain.Invariants;

public class Invariant
{
    private readonly List<Error> _violations = new();
    
    public static Invariant Create => new();

    public void AddViolation(Error error)
    {
        _violations.Add(error);
    }

    public Result<T> ValidateAndCreate<T>(Func<T> constructor)
    {
        if (!HasViolations()) 
            return Result<T>.Success(constructor());
        
        var error = Errors.Validation.InvariantViolationError(nameof(T), FormattedViolations());
        return Result<T>.Failure(error);
    }

    private bool HasViolations() => _violations.Count > 0;

    private IEnumerable<string> FormattedViolations() => _violations.Select(violation => violation.Message);
}