using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Infrastructure.Services;

namespace Zzaia.CoffeeShop.Order.Tests.Infrastructure.Services;

/// <summary>
/// Contains unit tests for PaymentService.
/// </summary>
public class PaymentServiceTests
{
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly Mock<ILogger<PaymentService>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;

    public PaymentServiceTests()
    {
        _mockCache = new Mock<IDistributedCache>();
        _mockLogger = new Mock<ILogger<PaymentService>>();
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5100")
        };
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithValidRequest_ReturnsSuccessResult()
    {
        PaymentRequest request = new("order-123", 100.50m, "USD", "user-456");
        string expectedTransactionId = "txn-789";
        object responseContent = new { TransactionId = expectedTransactionId };
        string jsonResponse = JsonSerializer.Serialize(responseContent);
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });
        _mockCache
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockCache
            .Setup(c => c.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        PaymentService service = new(_httpClient, _mockCache.Object, _mockLogger.Object);
        PaymentResult result = await service.ProcessPaymentAsync(request);
        result.Success.Should().BeTrue();
        result.TransactionId.Should().Be(expectedTransactionId);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithDistributedLockFailure_ReturnsErrorResult()
    {
        PaymentRequest request = new("order-123", 100.50m, "USD", "user-456");
        _mockCache
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Cache unavailable"));
        PaymentService service = new(_httpClient, _mockCache.Object, _mockLogger.Object);
        PaymentResult result = await service.ProcessPaymentAsync(request);
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("already being processed");
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithHttpFailure_ReturnsErrorResult()
    {
        PaymentRequest request = new("order-123", 100.50m, "USD", "user-456");
        string errorMessage = "Payment gateway unavailable";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent(errorMessage)
            });
        _mockCache
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockCache
            .Setup(c => c.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        PaymentService service = new(_httpClient, _mockCache.Object, _mockLogger.Object);
        PaymentResult result = await service.ProcessPaymentAsync(request);
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain(errorMessage);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithException_ReturnsErrorResult()
    {
        PaymentRequest request = new("order-123", 100.50m, "USD", "user-456");
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));
        _mockCache
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockCache
            .Setup(c => c.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        PaymentService service = new(_httpClient, _mockCache.Object, _mockLogger.Object);
        PaymentResult result = await service.ProcessPaymentAsync(request);
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Network error");
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReleasesLock_AfterProcessing()
    {
        PaymentRequest request = new("order-123", 100.50m, "USD", "user-456");
        string lockKey = $"payment:lock:{request.OrderId}";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"TransactionId\":\"txn-123\"}")
            });
        _mockCache
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockCache
            .Setup(c => c.RemoveAsync(
                lockKey,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        PaymentService service = new(_httpClient, _mockCache.Object, _mockLogger.Object);
        await service.ProcessPaymentAsync(request);
        _mockCache.Verify(
            c => c.RemoveAsync(
                lockKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RefundPaymentAsync_WithValidTransaction_ReturnsSuccessResult()
    {
        string transactionId = "txn-123";
        decimal amount = 50.25m;
        string currency = "USD";
        string refundTransactionId = "refund-456";
        object responseContent = new { TransactionId = refundTransactionId };
        string jsonResponse = JsonSerializer.Serialize(responseContent);
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });
        PaymentService service = new(_httpClient, _mockCache.Object, _mockLogger.Object);
        PaymentResult result = await service.RefundPaymentAsync(
            transactionId,
            amount,
            currency);
        result.Success.Should().BeTrue();
        result.TransactionId.Should().Be(refundTransactionId);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task RefundPaymentAsync_WithHttpFailure_ReturnsErrorResult()
    {
        string transactionId = "txn-123";
        decimal amount = 50.25m;
        string currency = "USD";
        string errorMessage = "Original transaction not found";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(errorMessage)
            });
        PaymentService service = new(_httpClient, _mockCache.Object, _mockLogger.Object);
        PaymentResult result = await service.RefundPaymentAsync(
            transactionId,
            amount,
            currency);
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain(errorMessage);
    }

    [Fact]
    public async Task RefundPaymentAsync_WithException_ReturnsErrorResult()
    {
        string transactionId = "txn-123";
        decimal amount = 50.25m;
        string currency = "USD";
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));
        PaymentService service = new(_httpClient, _mockCache.Object, _mockLogger.Object);
        PaymentResult result = await service.RefundPaymentAsync(
            transactionId,
            amount,
            currency);
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Network error");
    }

    [Fact]
    public async Task RefundPaymentAsync_SendsNegativeAmount_InRequest()
    {
        string transactionId = "txn-123";
        decimal amount = 50.25m;
        string currency = "USD";
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
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"TransactionId\":\"refund-456\"}")
            });
        PaymentService service = new(_httpClient, _mockCache.Object, _mockLogger.Object);
        await service.RefundPaymentAsync(transactionId, amount, currency);
        capturedRequest.Should().NotBeNull();
        string requestContent = await capturedRequest!.Content!.ReadAsStringAsync();
        requestContent.Should().Contain("-50.25");
    }
}
