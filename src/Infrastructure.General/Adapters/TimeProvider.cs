using Application.Ports.General;

namespace Infrastructure.General.Adapters;

public class TimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}