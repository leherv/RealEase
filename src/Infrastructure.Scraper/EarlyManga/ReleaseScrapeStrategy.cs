﻿using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Infrastructure.Scraper.Shared;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.EarlyManga;

internal class ReleaseScrapeStrategy : IReleaseScrapeStrategy
{
    public async Task<Result<ScrapedMediaRelease>> Execute(IPage page, ScrapeInstruction scrapeInstruction)
    {
        var container = await page.WaitForSelectorAsync("div.chapter-container", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeFailedError("Element containing chapters did not appear.");

        var newestLink = await container.QuerySelectorAsync("div.chapter-row >> div.col >> a");
        if (newestLink == null)
            return Errors.Scraper.ScrapeFailedError("Newest chapter link element could not be selected.");

        var relativeChapterUrl = await newestLink.GetAttributeAsync("href");
        if (relativeChapterUrl == null)
            return Errors.Scraper.ScrapeFailedError("Newest link element does not contain a chapter url");

        var releaseNumbersResult = ReleaseNumberExtractor.ExtractReleaseNumbers(relativeChapterUrl);
        if (releaseNumbersResult.IsFailure)
            return releaseNumbersResult.Error;

        return new ScrapedMediaRelease(
            UriCombinator.Combine(scrapeInstruction.WebsiteUrl, relativeChapterUrl),
            releaseNumbersResult.Value.Major,
            releaseNumbersResult.Value.Minor
        );
    }
}