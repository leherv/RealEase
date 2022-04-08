using Application.UseCases.Base;

namespace Application.UseCases.Media;

public record AvailableMedia(IReadOnlyCollection<string> MediaNames) : IReadModel;