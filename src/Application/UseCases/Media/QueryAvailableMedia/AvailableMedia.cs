using Application.UseCases.Base;

namespace Application.UseCases.Media.QueryAvailableMedia;

public record AvailableMedia(IReadOnlyCollection<string> MediaNames) : IReadModel;