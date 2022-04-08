
using Domain.ApplicationErrors;

namespace Domain.Results.Extensions;

public static class ResultExtensions
{
   public static Result OnFailure(this Result result, Action<Error> action)
   {
      if (result.IsFailure)
      {
         action(result.Error);
      }

      return result;
   }
}