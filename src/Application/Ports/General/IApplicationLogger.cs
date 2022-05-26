
namespace Application.Ports.General;

public interface IApplicationLogger
{
    void LogWarning(string message);
    void LogWarning(Exception e, string message);
}