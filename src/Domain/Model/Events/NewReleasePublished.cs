using Domain.Model.Base;

namespace Domain.Model.Events;

public record NewReleasePublished(string Link, string MediaName) : IDomainEvent;