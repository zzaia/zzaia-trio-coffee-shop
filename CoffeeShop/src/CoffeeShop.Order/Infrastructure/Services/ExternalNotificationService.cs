using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Services;

/// <summary>
/// Implements notification service operations with resilience patterns.
/// </summary>
public sealed class ExternalNotificationService(
    HttpClient httpClient,
    ILogger<ExternalNotificationService> logger) : INotificationService
{
    private readonly AsyncPolicy<HttpResponseMessage> _resiliencePolicy = CreateResiliencePolicy(logger);

    /// <inheritdoc/>
    public async Task<bool> SendOrderStatusNotificationAsync(
        string userId,
        Guid orderId,
        string status,
        CancellationToken cancellationToken = default)
    {
        try
        {
            object notification = new
            {
                status = status
            };
            HttpResponseMessage response = await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    "/api/v1/notification",
                    notification,
                    cancellationToken);
            });
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogInformation(
                    "Notification sent for order {OrderId} status {Status}. Response: {Response}",
                    orderId,
                    status,
                    responseContent);
                return true;
            }
            else
            {
                logger.LogWarning(
                    "Failed to send notification for order {OrderId}",
                    orderId);
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Exception sending notification for order {OrderId}",
                orderId);
            return false;
        }
    }

    private static AsyncPolicy<HttpResponseMessage> CreateResiliencePolicy(
        ILogger<ExternalNotificationService> logger)
    {
        AsyncPolicy<HttpResponseMessage> circuitBreaker = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                5,
                TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    logger.LogWarning(
                        "Notification service circuit breaker opened for {Duration}s",
                        duration.TotalSeconds);
                },
                onReset: () =>
                {
                    logger.LogInformation("Notification service circuit breaker reset");
                });
        AsyncPolicy<HttpResponseMessage> retry = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (result, timespan, retryCount, context) =>
                {
                    logger.LogWarning(
                        "Notification service retry {RetryCount} after {Delay}ms",
                        retryCount,
                        timespan.TotalMilliseconds);
                });
        AsyncPolicy<HttpResponseMessage> timeout = Policy
            .TimeoutAsync<HttpResponseMessage>(30);
        return Policy.WrapAsync(circuitBreaker, retry, timeout);
    }
}
