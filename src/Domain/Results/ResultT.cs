using Domain.ApplicationErrors;
using Domain.Results.Exceptions;

namespace Domain.Results;

public record Result<T>
{
    public bool IsFailure { get; }

    private readonly Error _error;

    private readonly T _value;
    public bool IsSuccess => !IsFailure;

    public Error Error => IsFailure
        ? _error
        : throw new ErrorInAccessibleException();

    public T Value => IsSuccess
        ? _value
        : throw new ResultInAccessibleException();

    private Result(bool isFailure, Error error, T? value)
    {
        IsFailure = isFailure;
        _error = error;
        _value = value;
    }

    public static Result<T> Success(T? value = default)
    {
        return new Result<T>(false, default, value);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(true, error, default);
    }
    
    public static implicit operator Result<T>(T value)
    {
        if (value is Result<T> result)
        {
            var resultError = result.IsFailure ? result.Error : default;
            var resultValue = result.IsSuccess ? result.Value : default;

            return new Result<T>(result.IsFailure, resultError, resultValue);
        }

        return Success(value);
    }
    
    public static implicit operator Result(Result<T> result)
    {
        if (result.IsSuccess)
            return Result.Success();
        else
            return Result.Failure(result.Error);
    }
    
    public static Result<T> SuccessIf(bool isSuccess, T value, Error error)
    {
        return isSuccess
            ? Success(value)
            : Failure(error);
    }
}