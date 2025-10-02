namespace Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

/// <summary>
/// Defines identity service client operations for user information retrieval.
/// </summary>
public interface IIdentityServiceClient
{
    /// <summary>
    /// Retrieves user information asynchronously.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user information if found, otherwise null.</returns>
    Task<UserInfo?> GetUserInfoAsync(
        string userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents user information from identity service.
/// </summary>
/// <param name="UserId">The user identifier.</param>
/// <param name="Email">The user email address.</param>
/// <param name="FullName">The user full name.</param>
/// <param name="Role">The user role.</param>
public record UserInfo(
    string UserId,
    string Email,
    string FullName,
    string Role);
