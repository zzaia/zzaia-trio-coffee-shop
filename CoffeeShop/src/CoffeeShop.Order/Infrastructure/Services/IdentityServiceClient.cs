using Dapr.Client;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Services;

/// <summary>
/// Implements identity service client operations using Dapr.
/// </summary>
public sealed class IdentityServiceClient(
    DaprClient daprClient,
    ILogger<IdentityServiceClient> logger) : IIdentityServiceClient
{
    /// <inheritdoc/>
    public async Task<UserInfo?> GetUserInfoAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            UserInfo? userInfo = await daprClient.InvokeMethodAsync<UserInfo>(
                "coffee-shop-identity",
                $"api/users/{userId}",
                cancellationToken);
            logger.LogInformation("Retrieved user info for {UserId}", userId);
            return userInfo;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to retrieve user info for {UserId}",
                userId);
            return null;
        }
    }
}
