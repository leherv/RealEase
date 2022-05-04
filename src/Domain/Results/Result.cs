using Domain.ApplicationErrors;
using Domain.Results.Exceptions;

namespace Domain.Results;

public record Result
{
    public bool IsFailure { get; }
    private readonly Error _error;
    public bool IsSuccess => !IsFailure;

    public Error Error => IsFailure
        ? _error
        : throw new ErrorInAccessibleException();

    private Result(bool isFailure, Error error)
    {
        IsFailure = isFailure;
        _error = error;
    }
    
    public static Result Success()
    {
        return new Result(false, default!);
    }

    public static Result Failure(Error error)
    {
        return new Result(true, error);
    }
    
    public static Result SuccessIf(bool isSuccess, Error error)
    {
        return isSuccess
            ? Success()
            : Failure(error);
    }
    
    public static implicit operator Result(Error error)
    {
        return Failure(error);
    }
}