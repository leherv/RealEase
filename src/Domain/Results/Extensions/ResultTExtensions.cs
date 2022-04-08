using Domain.ApplicationErrors;

namespace Domain.Results.Extensions;

public static class ResultTExtensions
{
    public static async Task<Result<T>> ToResult<T>(this Task<T?> maybeTask, Error error)
    {
        var maybe = await maybeTask;
        return Result<T>.SuccessIf(maybe != null, maybe, error);
    }
    
    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> resultTask, Action<T> action)
    {
        Result<T> result = await resultTask;
        return result.Tap(action);
    }
    
    public static Result<T> Tap<T>(this Result<T> result, Action action)
    {
        if (result.IsSuccess)
            action();

        return result;
    }

    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);

        return result;
    }
}