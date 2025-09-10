namespace QuickChat.Gateway.Services;

public interface IServiceHttpClient
{
    Task<TResponse> InvokeRequest<TResponse>(
        HttpRequestMessage request,
        bool isSensitiveData = false
    );
}
