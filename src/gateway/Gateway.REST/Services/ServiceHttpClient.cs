using System.Text.Json;
using QuickChat.Gateway.REST.Exceptions;
using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Services;

public class ServiceHttpClient(HttpClient client, ILogger<ServiceHttpClient> logger)
    : IServiceHttpClient
{
    private readonly HttpClient client = client;
    private readonly ILogger<ServiceHttpClient> logger = logger;
    private static readonly JsonSerializerOptions jsonSerializerOptions =
        new() { PropertyNameCaseInsensitive = true };

    public async Task<TResponse> InvokeRequest<TResponse>(
        HttpRequestMessage request,
        bool isSensitiveData = false
    )
    {
        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(request);
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, "Failed to send request to a service");
            throw;
        }

        string rawData = await response.Content.ReadAsStringAsync();

        logger.LogInformation(
            "{method} {url} sends {statusCode}:\n {data}",
            request.Method,
            request.RequestUri,
            response.StatusCode,
            response.IsSuccessStatusCode && isSensitiveData ? "***" : rawData
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException(
                JsonSerializer.Deserialize<ApiExceptionModel>(rawData, jsonSerializerOptions)!
            );
        }

        return JsonSerializer.Deserialize<TResponse>(rawData, jsonSerializerOptions)!;
    }
}
