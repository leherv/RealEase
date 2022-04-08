namespace Domain.Results.Exceptions;

internal class ResultInAccessibleException : Exception
{
    private const string ValueIsInaccessibleForError = "You attempted to access the Value property for a failure result. A failure result has no Value.";
    internal ResultInAccessibleException()
        : base(ValueIsInaccessibleForError)
    {
    }
}