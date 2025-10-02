using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Infrastructure.Services;

namespace Zzaia.CoffeeShop.Order.Tests.Infrastructure.Services;

/// <summary>
/// Contains unit tests for ExternalNotificationService.
/// </summary>
public class ExternalNotificationServiceTests
{
    private readonly Mock<ILogger<ExternalNotificationService>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;

    public ExternalNotificationServiceTests()
    {
        _mockLogger = new Mock<ILogger<ExternalNotificationService>>();
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5200")
        };
    }

    [Fact]
    public async Task SendOrderStatusNotificationAsync_WithValidRequest_ReturnsTrue()
    {
        string userId = "user-123";
        Guid orderId = Guid.NewGuid();
        string status = "Confirmed";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });
        ExternalNotificationService service = new(_httpClient, _mockLogger.Object);
        bool result = await service.SendOrderStatusNotificationAsync(
            userId,
            orderId,
            status);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SendOrderStatusNotificationAsync_WithHttpFailure_ReturnsFalse()
    {
        string userId = "user-123";
        Guid orderId = Guid.NewGuid();
        string status = "Confirmed";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            });
        ExternalNotificationService service = new(_httpClient, _mockLogger.Object);
        bool result = await service.SendOrderStatusNotificationAsync(
            userId,
            orderId,
            status);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SendOrderStatusNotificationAsync_WithException_ReturnsFalse()
    {
        string userId = "user-123";
        Guid orderId = Guid.NewGuid();
        string status = "Confirmed";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));
        ExternalNotificationService service = new(_httpClient, _mockLogger.Object);
        bool result = await service.SendOrderStatusNotificationAsync(
            userId,
            orderId,
            status);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SendOrderStatusNotificationAsync_SendsCorrectPayload()
    {
        string userId = "user-123";
        Guid orderId = Guid.NewGuid();
        string status = "Confirmed";
        HttpRequestMessage? capturedRequest = null;
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) =>
            {
                capturedRequest = req;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });
        ExternalNotificationService service = new(_httpClient, _mockLogger.Object);
        await service.SendOrderStatusNotificationAsync(userId, orderId, status);
        capturedRequest.Should().NotBeNull();
        string requestContent = await capturedRequest!.Content!.ReadAsStringAsync();
        requestContent.Should().Contain(status);
    }

    [Fact]
    public async Task SendOrderStatusNotificationAsync_UsesCorrectEndpoint()
    {
        string userId = "user-123";
        Guid orderId = Guid.NewGuid();
        string status = "Confirmed";
        HttpRequestMessage? capturedRequest = null;
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) =>
            {
                capturedRequest = req;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });
        ExternalNotificationService service = new(_httpClient, _mockLogger.Object);
        await service.SendOrderStatusNotificationAsync(userId, orderId, status);
        capturedRequest.Should().NotBeNull();
        capturedRequest!.RequestUri!.ToString()
            .Should().Contain("/api/v1/notification");
    }

    [Fact]
    public async Task SendOrderStatusNotificationAsync_WithServerError_ReturnsFalse()
    {
        string userId = "user-123";
        Guid orderId = Guid.NewGuid();
        string status = "Confirmed";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });
        ExternalNotificationService service = new(_httpClient, _mockLogger.Object);
        bool result = await service.SendOrderStatusNotificationAsync(
            userId,
            orderId,
            status);
        result.Should().BeFalse();
    }
}
