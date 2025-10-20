using System.Text.Json.Serialization;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Services;

/// <summary>
/// Implements token acquisition using client credentials flow.
/// </summary>
public sealed class TokenService(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<TokenService> logger) : ITokenService
{
    private string? _cachedToken;
    private DateTimeOffset _tokenExpiration = DateTimeOffset.MinValue;

    /// <inheritdoc/>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(_cachedToken) && DateTimeOffset.UtcNow < _tokenExpiration)
        {
            return _cachedToken;
        }
        try
        {
            string tokenEndpoint = configuration["Authentication:TokenEndpoint"]
                ?? throw new InvalidOperationException("Token endpoint not configured");
            string clientId = configuration["Authentication:ClientId"]
                ?? throw new InvalidOperationException("Client ID not configured");
            string clientSecret = configuration["Authentication:ClientSecret"]
                ?? throw new InvalidOperationException("Client secret not configured");
            Dictionary<string, string> requestData = new()
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["scope"] = "order-api"
            };
            FormUrlEncodedContent content = new(requestData);
            HttpResponseMessage response = await httpClient.PostAsync(
                tokenEndpoint,
                content,
                cancellationToken);
            response.EnsureSuccessStatusCode();
            TokenResponse? tokenResponse = await response.Content
                .ReadFromJsonAsync<TokenResponse>(cancellationToken);
            if (tokenResponse is null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Failed to acquire access token");
            }
            _cachedToken = tokenResponse.AccessToken;
            _tokenExpiration = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);
            logger.LogInformation("Access token acquired, expires at {Expiration}", _tokenExpiration);
            return _cachedToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to acquire access token");
            throw;
        }
    }

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("token_type")] string TokenType,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);
}
