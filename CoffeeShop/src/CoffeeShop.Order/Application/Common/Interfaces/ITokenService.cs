namespace Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

/// <summary>
/// Defines token acquisition operations for service-to-service authentication.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Acquires an access token using client credentials flow.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
