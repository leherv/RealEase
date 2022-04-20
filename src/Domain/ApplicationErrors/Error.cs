namespace Domain.ApplicationErrors;

public sealed record Error
{
    public string Code { get; }
    public string Message { get; }
    public IReadOnlyCollection<string> Details { get; }

    public Error(
        string code,
        string message,
        IEnumerable<string>? details = null
    )
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("Code must not be empty", nameof(code));

        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Message must not be empty", nameof(code));

        Code = code;
        Message = message;
        Details = details?.ToList() ?? new List<string>();
    }

    public override string ToString()
    {
        var result = $"Error {Code}: {Message}";
        if (HasDetails)
            result += $"\n{FormattedDetails}";
        
        return result;
    }

    private string FormattedDetails => string.Join('\n', Details);

    private bool HasDetails => Details.Count > 0;
}