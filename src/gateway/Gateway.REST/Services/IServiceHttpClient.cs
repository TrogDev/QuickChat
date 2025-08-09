namespace QuickChat.Gateway.REST.Services;

public interface IServiceHttpClient
{
    Task<TResponse> InvokeRequest<TResponse>(
        HttpRequestMessage request,
        bool isSensitiveData = false
    );
}
