using Domain.Results;

namespace Application.Test.Fixture.Thens;

public class ThenTheResult
{
    private readonly Result _result;

    public ThenTheResult(Result result)
    {
        this._result = result;
    }

    public bool IsSuccessful()
    {
        return _result.IsSuccess;
    }

    public bool IsAFailure()
    {
        return _result.IsFailure;
    }

    public bool ContainsError(string code)
    {
        return _result.Error.Code.Equals(code);
    }
    
    
}