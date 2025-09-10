using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Services;

public interface IIdentityService
{
    Task<UserCredentialsModel> CreateAnonymousUser();
}
