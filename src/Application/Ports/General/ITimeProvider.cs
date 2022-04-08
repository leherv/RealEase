namespace Application.Ports.General;

public interface ITimeProvider
{ 
    DateTime UtcNow { get; }
}