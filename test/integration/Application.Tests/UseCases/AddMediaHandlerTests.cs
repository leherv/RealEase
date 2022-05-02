using System.Threading.Tasks;
using Application.Test.Fixture;
using Application.UseCases.Media.AddMedia;
using Domain.ApplicationErrors;
using Domain.Results;
using FluentAssertions;
using Xunit;

namespace Application.Test.UseCases;

public class AddMediaHandlerTests : IntegrationTestBase
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Adding_media_fails_if_website_does_not_exist()
    {
        await Given.TheDatabase.IsSeeded();
        var addMediaCommand = new AddMediaCommand("does not exist");

        var addMediaResult = await When.TheApplication.ReceivesCommand<AddMediaCommand, Result>(addMediaCommand);

        Then.TheResult(addMediaResult)
            .IsAFailure()
            .Should()
            .BeTrue();
        Then.TheResult(addMediaResult).ContainsErrorWithCode(Errors.General.NotFoundErrorCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Adding_media_fails_if_scraping_fails()
    {
        await Given.TheDatabase.IsSeeded();
        var addMediaCommand = new AddMediaCommand(
            Given.The.Website.EarlyManga.Name
        );

        var addMediaResult = await When.TheApplication.ReceivesCommand<AddMediaCommand, Result>(addMediaCommand);

        Then.TheResult(addMediaResult)
            .IsAFailure()
            .Should()
            .BeTrue();
    }
}