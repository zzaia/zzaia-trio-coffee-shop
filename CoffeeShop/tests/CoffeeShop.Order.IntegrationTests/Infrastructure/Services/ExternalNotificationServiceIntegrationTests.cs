using FluentAssertions;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Infrastructure.Services;

namespace Zzaia.CoffeeShop.Order.IntegrationTests.Infrastructure.Services;

/// <summary>
/// Integration tests for ExternalNotificationService making real HTTP calls to Trio Challenge API.
/// </summary>
[Trait("Category", "Integration")]
public sealed class ExternalNotificationServiceIntegrationTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalNotificationService> _logger;
    private readonly INotificationService _notificationService;

    public ExternalNotificationServiceIntegrationTests()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://challenge.trio.dev"),
            Timeout = TimeSpan.FromSeconds(30)
        };
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        _logger = loggerFactory.CreateLogger<ExternalNotificationService>();
        _notificationService = new ExternalNotificationService(_httpClient, _logger);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _httpClient?.Dispose();
        return Task.CompletedTask;
    }

    [Fact(Skip = "Requires real Trio Challenge API")]
    public async Task SendOrderStatusNotificationAsync_WithRealAPI_ShouldSucceed()
    {
        string userId = "test-user-789";
        Guid orderId = Guid.NewGuid();
        string status = "Preparation";
        bool result = await _notificationService.SendOrderStatusNotificationAsync(
            userId,
            orderId,
            status
        );
        result.Should().BeTrue();
        Console.WriteLine($"Notification sent successfully for order {orderId} with status {status}");
    }

    [Theory(Skip = "Requires real Trio Challenge API")]
    [InlineData("Waiting")]
    [InlineData("Preparation")]
    [InlineData("Ready")]
    [InlineData("Delivered")]
    public async Task SendOrderStatusNotificationAsync_WithAllStatuses_ShouldSucceed(string status)
    {
        string userId = "test-user-multiple";
        Guid orderId = Guid.NewGuid();
        bool result = await _notificationService.SendOrderStatusNotificationAsync(
            userId,
            orderId,
            status
        );
        result.Should().BeTrue();
        Console.WriteLine($"Notification sent for status: {status}");
    }
}
