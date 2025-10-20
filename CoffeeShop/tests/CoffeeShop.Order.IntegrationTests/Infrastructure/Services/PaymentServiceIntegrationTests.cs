using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Infrastructure.Services;

namespace Zzaia.CoffeeShop.Order.IntegrationTests.Infrastructure.Services;

/// <summary>
/// Integration tests for PaymentService making real HTTP calls to Trio Challenge API.
/// </summary>
[Trait("Category", "Integration")]
public sealed class PaymentServiceIntegrationTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<PaymentService> _logger;
    private readonly IPaymentService _paymentService;

    public PaymentServiceIntegrationTests()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://challenge.trio.dev"),
            Timeout = TimeSpan.FromSeconds(30)
        };
        RedisCacheOptions redisOptions = new()
        {
            Configuration = "localhost:6379"
        };
        _distributedCache = new RedisCache(Options.Create(redisOptions));
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        _logger = loggerFactory.CreateLogger<PaymentService>();
        _paymentService = new PaymentService(_httpClient, _distributedCache, _logger);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _httpClient?.Dispose();
        if (_distributedCache is IDisposable disposable)
        {
            disposable.Dispose();
        }
        await Task.CompletedTask;
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithRealAPI_ShouldSucceed()
    {
        PaymentRequest request = new(
            OrderId: Guid.NewGuid().ToString(),
            Amount: 12.50m,
            Currency: "USD",
            UserId: "test-user-123"
        );
        PaymentResult result = await _paymentService.ProcessPaymentAsync(request);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.TransactionId.Should().NotBeNullOrEmpty();
        result.ErrorMessage.Should().BeNull();
        Console.WriteLine($"Payment successful! Transaction ID: {result.TransactionId}");
    }

    [Fact]
    public async Task RefundPaymentAsync_WithRealAPI_ShouldSucceed()
    {
        PaymentRequest paymentRequest = new(
            OrderId: Guid.NewGuid().ToString(),
            Amount: 10.00m,
            Currency: "USD",
            UserId: "test-user-456"
        );
        PaymentResult paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);
        paymentResult.Success.Should().BeTrue();
        string transactionId = paymentResult.TransactionId!;
        PaymentResult refundResult = await _paymentService.RefundPaymentAsync(
            transactionId,
            10.00m,
            "USD"
        );
        refundResult.Should().NotBeNull();
        refundResult.Success.Should().BeTrue();
        refundResult.TransactionId.Should().NotBeNullOrEmpty();
        refundResult.ErrorMessage.Should().BeNull();
        Console.WriteLine($"Refund successful! Transaction ID: {refundResult.TransactionId}");
    }
}
