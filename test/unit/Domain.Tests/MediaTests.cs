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
}