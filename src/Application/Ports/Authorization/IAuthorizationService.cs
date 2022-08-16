namespace Application.Ports.Authorization;

public interface IAuthorizationService
{ 
    bool IsAdmin(string externalIdentifier);
}