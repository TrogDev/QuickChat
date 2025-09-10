using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using QuickChat.Gateway.Exceptions;
using QuickChat.Gateway.IntegrationTests.Models;
using QuickChat.Gateway.Models;
using QuickChat.Gateway.Services;

namespace QuickChat.Gateway.IntegrationTests;

public class ServiceHttpClientTests
{
    [Fact]
    public async Task InvokeRequest_WithSuccessfulResponse_ReturnsDeserializedResponse()
    {
        // Arrange
        TestModel expectedResponse = new("Test");
        string jsonResponse = JsonSerializer.Serialize(expectedResponse);

        Mock<HttpMessageHandler> handlerMock = new(MockBehavior.Strict);

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                }
            );

        HttpClient httpClient = new(handlerMock.Object);
        ILogger<ServiceHttpClient> loggerMock = Mock.Of<ILogger<ServiceHttpClient>>();
        ServiceHttpClient serviceClient = new(httpClient, loggerMock);

        HttpRequestMessage request = new(HttpMethod.Get, "https://example.com");

        // Act
        TestModel result = await serviceClient.InvokeRequest<TestModel>(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Message, result.Message);
    }

    [Fact]
    public async Task InvokeRequest_WithFailureResponse_ThrowsApiException()
    {
        // Arrange
        ApiExceptionModel errorResponse =
            new(404, "EntityNotFoundException", "Title", "Description");
        string jsonError = JsonSerializer.Serialize(errorResponse);

        Mock<HttpMessageHandler> handlerMock = new(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(jsonError)
                }
            );

        HttpClient httpClient = new(handlerMock.Object);
        ILogger<ServiceHttpClient> loggerMock = Mock.Of<ILogger<ServiceHttpClient>>();
        ServiceHttpClient serviceClient = new(httpClient, loggerMock);

        HttpRequestMessage request = new(HttpMethod.Get, "https://example.com");

        // Act & Assert
        await Assert.ThrowsAsync<ApiException>(
            () => serviceClient.InvokeRequest<TestModel>(request)
        );
    }
}
