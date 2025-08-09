using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Services;

public interface IIdentityService
{
    Task<UserCredentialsModel> CreateAnonymousUser();
}
