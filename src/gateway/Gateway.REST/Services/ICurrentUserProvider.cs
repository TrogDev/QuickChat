namespace QuickChat.Gateway.REST.Services;

public interface ICurrentUserProvider
{
    public Guid GetCurrentUserId();
}
