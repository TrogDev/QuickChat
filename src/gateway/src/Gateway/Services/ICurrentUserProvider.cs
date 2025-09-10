namespace QuickChat.Gateway.Services;

public interface ICurrentUserProvider
{
    public Guid GetCurrentUserId();
}
