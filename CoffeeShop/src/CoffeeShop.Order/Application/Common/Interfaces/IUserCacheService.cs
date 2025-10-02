namespace Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

/// <summary>
/// Defines user cache operations.
/// </summary>
public interface IUserCacheService
{
    /// <summary>
    /// Creates or updates a user in the local cache.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="email">The user email.</param>
    /// <param name="fullName">The user full name.</param>
    /// <param name="role">The user role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateOrUpdateUserAsync(
        string userId,
        string email,
        string fullName,
        string role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user from the local cache.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
}
