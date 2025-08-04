namespace QuickChat.Identity.Application.Services;

public interface ITokenService
{
    string CreateAccessToken(Guid userId);
}
