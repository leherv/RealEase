namespace Domain.Results.Exceptions;

internal class ErrorInAccessibleException : Exception
{
    private const string ErrorIsInaccessibleForSuccess = "You attempted to access the Error property for a successful result. A successful result has no Error.";

    internal ErrorInAccessibleException()
        : base(ErrorIsInaccessibleForSuccess)
    {
    }
}