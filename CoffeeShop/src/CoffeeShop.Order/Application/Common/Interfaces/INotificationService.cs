namespace Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

/// <summary>
/// Defines notification service operations for order status updates.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends order status notification to user asynchronously.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="status">The order status.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if notification was sent successfully, otherwise false.</returns>
    Task<bool> SendOrderStatusNotificationAsync(
        string userId,
        Guid orderId,
        string status,
        CancellationToken cancellationToken = default);
}
