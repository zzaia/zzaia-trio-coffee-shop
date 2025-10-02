using System.Net.Http.Headers;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Services;

/// <summary>
/// Implements identity service client operations using Dapr service invocation.
/// </summary>
public sealed class IdentityServiceClient(
    DaprClient daprClient,
    ITokenService tokenService,
    ILogger<IdentityServiceClient> logger) : IIdentityServiceClient
{
    private const string IdentityAppId = "identity-service";

    /// <inheritdoc/>
    public async Task<UserInfo?> GetUserInfoAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string accessToken = await tokenService.GetAccessTokenAsync(cancellationToken);
            HttpRequestMessage request = daprClient.CreateInvokeMethodRequest(
                HttpMethod.Get,
                IdentityAppId,
                $"api/users/{userId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await daprClient.InvokeMethodWithResponseAsync(
                request,
                cancellationToken);
            response.EnsureSuccessStatusCode();
            UserInfo? userInfo = await response.Content.ReadFromJsonAsync<UserInfo>(cancellationToken);
            logger.LogInformation("Retrieved user info for {UserId} via Dapr service invocation", userId);
            return userInfo;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to retrieve user info for {UserId} via Dapr service invocation",
                userId);
            return null;
        }
    }
}
