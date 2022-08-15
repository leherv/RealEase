using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Website.QueryAvailableWebsites;
using Application.UseCases.Website.SetInactive;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class SetWebsiteInactiveTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Fails_if_Website_does_not_exist()
    {
        var setWebsiteInactiveCommand = new SetWebsiteInactiveCommand(
            Guid.NewGuid()
        );

        var setWebsiteInactiveResult = await When.TheApplication
            .ReceivesCommand<SetWebsiteInactiveCommand, Result>(setWebsiteInactiveCommand);

        Then.TheResult(setWebsiteInactiveResult)
            .IsSuccessful()
            .Should()
            .BeFalse();
        Then.TheResult(setWebsiteInactiveResult)
            .ContainsErrorWithCode(Errors.General.NotFoundErrorCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Website_can_not_be_queried_if_inactive()
    {
        await Given.TheDatabase.IsSeeded();
        var websites = Given.The.Website.ActiveWebsites;
        var websiteToSetInactive = websites.First();
        var setWebsiteInactiveCommand = new SetWebsiteInactiveCommand(
            websiteToSetInactive.Id
        );

        var setWebsiteInactiveResult = await When.TheApplication
            .ReceivesCommand<SetWebsiteInactiveCommand, Result>(setWebsiteInactiveCommand);

        Then.TheResult(setWebsiteInactiveResult)
            .IsSuccessful()
            .Should()
            .BeTrue();

        var availableWebsites =
            await Then.TheApplication.ReceivesQuery<AvailableWebsitesQuery, AvailableWebsites>(
                new AvailableWebsitesQuery());
        availableWebsites.Websites.Should().HaveCount(websites.Count - 1);
    }
}