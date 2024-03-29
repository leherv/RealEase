using System;
using System.Linq;
using Domain.ApplicationErrors;
using Domain.Model;
using FluentAssertions;
using Shared;
using Xunit;

namespace Domain.Tests;

public class MediaTests
{
    [Fact]
    public void Publish_new_release_works_when_no_release_for_media_yet()
    {
        var media = GivenTheMedia.Create(mediaName: "Solo Leveling").Value;
        var release = GivenTheRelease.Create().Value;

        var result = media.PublishNewRelease(release);

        result.IsSuccess.Should().BeTrue();
        media.NewestRelease.Should().Be(release);
        media.DomainEvents.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(1, 0, 2, 0)]
    [InlineData(2, 0, 2, 1)]
    [InlineData(2, 9, 3, 0)]
    public void Publish_new_release_works_when_release_is_newer_than_current(
        int existingReleaseMajor,
        int existingReleaseMinor,
        int newReleaseMajor,
        int newReleaseMinor
    )
    {
        var media = GivenTheMedia.Create(mediaName: "Solo Leveling").Value;
        var existingRelease = GivenTheRelease.Create(
            ReleaseNumber.Create(existingReleaseMajor, existingReleaseMinor).Value
        ).Value;
        var newRelease = GivenTheRelease.Create(
            ReleaseNumber.Create(newReleaseMajor, newReleaseMinor).Value
        ).Value;
        media.PublishNewRelease(existingRelease);

        var result = media.PublishNewRelease(newRelease);

        result.IsSuccess.Should().BeTrue();
        media.NewestRelease.Should().Be(newRelease);
        media.DomainEvents.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(2, 0, 1, 0)]
    [InlineData(2, 1, 2, 0)]
    [InlineData(2, 0, 2, 0)]
    [InlineData(2, 1, 2, 1)]
    public void Publish_new_release_fails_when_release_not_newer(
        int existingReleaseMajor,
        int existingReleaseMinor,
        int newReleaseMajor,
        int newReleaseMinor
    )
    {
        var media = GivenTheMedia.Create(mediaName: "Solo Leveling").Value;
        var existingRelease = GivenTheRelease.Create(
            ReleaseNumber.Create(existingReleaseMajor, existingReleaseMinor).Value
        ).Value;
        var newRelease = GivenTheRelease.Create(
            ReleaseNumber.Create(newReleaseMajor, newReleaseMinor).Value
        ).Value;
        media.PublishNewRelease(existingRelease);

        var result = media.PublishNewRelease(newRelease);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(Errors.Media.PublishNewReleaseErrorCode);
        media.NewestRelease.Should().Be(existingRelease);
        media.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Adding_ScrapeTarget_succeeds_if_none_yet()
    {
        var media = GivenTheMedia.Create().Value;
        var scrapeTarget = GivenTheScrapeTarget.Create().Value;

        var result = media.AddScrapeTarget(scrapeTarget, media.Name);

        result.IsSuccess.Should().BeTrue();
        media.ScrapeTargets.Should().HaveCount(1);
        media.ScrapeTargets.First().Id.Should().Be(scrapeTarget.Id);
    }

    [Fact]
    public void Adding_ScrapeTarget_fails_if_already_added()
    {
        var media = GivenTheMedia.Create().Value;
        var website = GivenTheWebsite.Create().Value;
        var scrapeTarget = GivenTheScrapeTarget.Create(website: website).Value;
        var scrapeTarget2 = GivenTheScrapeTarget.Create(website: website).Value;

        media.AddScrapeTarget(scrapeTarget, media.Name);
        var result = media.AddScrapeTarget(scrapeTarget2, media.Name);

        result.IsSuccess.Should().BeFalse();
        media.ScrapeTargets.Should().HaveCount(1);
        media.ScrapeTargets.First().Id.Should().Be(scrapeTarget.Id);
    }
    
    [Theory]
    [InlineData("Boruto", "Hunter X Hunter")]
    [InlineData("Hunter X Hunter", "")]
    [InlineData("Naruto", "Boruto")]
    public void Adding_ScrapeTarget_fails_if_it_references_different_media(string mediaName, string scrapedMediaName)
    {
        var media = GivenTheMedia.Create(mediaName: mediaName).Value;
        var website = GivenTheWebsite.Create().Value;
        var scrapeTarget = GivenTheScrapeTarget.Create(website: website).Value;

        var result = media.AddScrapeTarget(scrapeTarget, scrapedMediaName);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(Errors.Media.ScrapeTargetReferencesOtherMediaErrorCode);
        media.ScrapeTargets.Should().BeEmpty();
    }
    
    [Theory]
    [InlineData("Boruto", "Boruto: Naruto Next Generations")]
    [InlineData("Boruto: Naruto Next Generations", "Boruto")]
    public void Adding_ScrapeTarget_succeeds_if_it_references_same_media(string mediaName, string scrapedMediaName)
    {
        var media = GivenTheMedia.Create(mediaName: mediaName).Value;
        var website = GivenTheWebsite.Create().Value;
        var scrapeTarget = GivenTheScrapeTarget.Create(website: website).Value;

        var result = media.AddScrapeTarget(scrapeTarget, scrapedMediaName);

        result.IsSuccess.Should().BeTrue();
        media.ScrapeTargets.Should().NotBeEmpty();
    }

    [Fact]
    public void Removing_ScrapeTarget_works_if_it_exists()
    {
        var media = GivenTheMedia.Create().Value;
        var scrapeTarget = GivenTheScrapeTarget.Create().Value;
        media.AddScrapeTarget(scrapeTarget, media.Name);

        var result = media.RemoveScrapeTarget(scrapeTarget.Id);

        result.Should().BeTrue();
        media.ScrapeTargets.Should().BeEmpty();
    }

    [Fact]
    public void Removing_ScrapeTarget_fails_if_it_does_not_exist()
    {
        var media = GivenTheMedia.Create().Value;
        var scrapeTarget = GivenTheScrapeTarget.Create().Value;
        media.AddScrapeTarget(scrapeTarget, media.Name);
        var scrapeTargetIdThatDoesNotExist = Guid.NewGuid();

        var result = media.RemoveScrapeTarget(scrapeTargetIdThatDoesNotExist);

        result.Should().BeFalse();
        media.ScrapeTargets.Should().NotBeEmpty();
    }

    [Fact]
    public void Removing_ScrapeTarget_fails_if_no_ScrapeTargets_yet()
    {
        var media = GivenTheMedia.Create().Value;
        var scrapeTargetIdThatDoesNotExist = Guid.NewGuid();

        var result = media.RemoveScrapeTarget(scrapeTargetIdThatDoesNotExist);

        result.Should().BeFalse();
        media.ScrapeTargets.Should().BeEmpty();
    }
}