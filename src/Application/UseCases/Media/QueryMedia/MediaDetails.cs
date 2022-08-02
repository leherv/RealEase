namespace Application.UseCases.Media.QueryMedia;

public record MediaDetails(Guid Id, string Name, IReadOnlyCollection<ScrapeTargetDetails> ScrapeTargetDetails, ReleaseDetails? ReleaseDetails);