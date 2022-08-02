using Application.UseCases.Base;

namespace Application.UseCases.Media.QueryAvailableMedia;

public record AvailableMedia(IReadOnlyCollection<MediaInformation> Media) : IReadModel;