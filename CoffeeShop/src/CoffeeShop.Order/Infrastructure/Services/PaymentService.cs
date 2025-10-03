using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Services;

/// <summary>
/// Implements payment service operations with resilience patterns.
/// </summary>
public sealed class PaymentService(
    HttpClient httpClient,
    IDistributedCache distributedCache,
    ILogger<PaymentService> logger) : IPaymentService
{
    private readonly AsyncPolicy<HttpResponseMessage> _resiliencePolicy = CreateResiliencePolicy(logger);

    /// <inheritdoc/>
    public async Task<PaymentResult> ProcessPaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        string lockKey = $"payment:lock:{request.OrderId}";
        try
        {
            bool lockAcquired = await TryAcquireLockAsync(
                distributedCache,
                lockKey,
                TimeSpan.FromSeconds(60),
                cancellationToken);
            if (!lockAcquired)
            {
                logger.LogWarning(
                    "Failed to acquire payment lock for order {OrderId}",
                    request.OrderId);
                return new PaymentResult(
                    false,
                    null,
                    "Payment is already being processed for this order");
            }
            object paymentPayload = new
            {
                value = request.Amount
            };
            HttpResponseMessage response = await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    "/api/v1/payment",
                    paymentPayload,
                    cancellationToken);
            });
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogInformation(
                    "Payment processed successfully for order {OrderId}, Response: {Response}",
                    request.OrderId,
                    responseContent);
                return new PaymentResult(
                    true,
                    Guid.NewGuid().ToString(),
                    null);
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "Payment failed for order {OrderId}: {Error}",
                    request.OrderId,
                    error);
                return new PaymentResult(false, null, error);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Exception processing payment for order {OrderId}",
                request.OrderId);
            return new PaymentResult(false, null, ex.Message);
        }
        finally
        {
            await ReleaseLockAsync(distributedCache, lockKey, logger, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<PaymentResult> RefundPaymentAsync(
        string transactionId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        try
        {
            object refundRequest = new
            {
                value = -amount,
                original_transaction_id = transactionId
            };
            HttpResponseMessage response = await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await httpClient.PostAsJsonAsync(
                    "/api/v1/payment",
                    refundRequest,
                    cancellationToken);
            });
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogInformation(
                    "Refund processed successfully for transaction {TransactionId}. Response: {Response}",
                    transactionId,
                    responseContent);
                return new PaymentResult(
                    true,
                    Guid.NewGuid().ToString(),
                    null);
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "Refund failed for transaction {TransactionId}: {Error}",
                    transactionId,
                    error);
                return new PaymentResult(false, null, error);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Exception processing refund for transaction {TransactionId}",
                transactionId);
            return new PaymentResult(false, null, ex.Message);
        }
    }

    private static AsyncPolicy<HttpResponseMessage> CreateResiliencePolicy(
        ILogger<PaymentService> logger)
    {
        AsyncPolicy<HttpResponseMessage> circuitBreaker = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                5,
                TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    logger.LogWarning(
                        "Payment service circuit breaker opened for {Duration}s",
                        duration.TotalSeconds);
                },
                onReset: () =>
                {
                    logger.LogInformation("Payment service circuit breaker reset");
                });
        AsyncPolicy<HttpResponseMessage> retry = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (result, timespan, retryCount, context) =>
                {
                    logger.LogWarning(
                        "Payment service retry {RetryCount} after {Delay}ms",
                        retryCount,
                        timespan.TotalMilliseconds);
                });
        AsyncPolicy<HttpResponseMessage> timeout = Policy
            .TimeoutAsync<HttpResponseMessage>(30);
        return Policy.WrapAsync(circuitBreaker, retry, timeout);
    }

    private static async Task<bool> TryAcquireLockAsync(
        IDistributedCache cache,
        string key,
        TimeSpan expiry,
        CancellationToken cancellationToken)
    {
        try
        {
            await cache.SetStringAsync(
                key,
                "locked",
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry
                },
                cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static async Task ReleaseLockAsync(
        IDistributedCache cache,
        string key,
        ILogger<PaymentService> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to release payment lock {Key}", key);
        }
    }

    private record PaymentResponse(string Success, string Message);
}
