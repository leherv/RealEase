using System.Text.RegularExpressions;
using Domain.ApplicationErrors;
using Domain.Results;

namespace Infrastructure.Scraper.Shared;

internal static class ReleaseNumberExtractor
{
    internal static Result<(int Major, int Minor)> ExtractReleaseNumbers(string value)
    {
        var majorResult = ExtractMajor(value);
        if (majorResult.IsFailure)
            return majorResult.Error; 
        var minorResult = ExtractMinor(value);
        if (minorResult.IsFailure)
            return minorResult.Error; 
        
        return Result<(int Major, int Minor)>.Success((majorResult.Value, minorResult.Value));
    
    }
    
    private static Result<int> ExtractMajor(string chapterUrl)
    {
        var regexResult = Regex.Match(chapterUrl, @"Episode (\d{1,4})");
        var majorString = regexResult.Groups[1].Value;
        if (!int.TryParse(majorString, out var major))
            return Errors.Scraper.ScrapeFailedError($"Major could not be extracted from url {chapterUrl}");
        
        return major;
    }
    
    private static Result<int> ExtractMinor(string chapterUrl)
    {
        var regexResult = Regex.Match(chapterUrl, @"Episode \d{1,4}[\-|.]*(\d{0,4})");
        var minorString = regexResult.Groups[1].Value;
        if (string.IsNullOrEmpty(minorString))
            return 0;
        
        if (!int.TryParse(minorString, out var minor))
        {
            return Errors.Scraper.ScrapeFailedError($"Minor could not be extracted from url {chapterUrl}");
        }

        return minor;
    }
}