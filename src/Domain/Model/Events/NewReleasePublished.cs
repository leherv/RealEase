using Domain.Model.Base;

namespace Domain.Model.Events;

public record NewReleasePublished(Guid MediaId, string MediaName, string LinkToReleasedResource) : IDomainEvent;