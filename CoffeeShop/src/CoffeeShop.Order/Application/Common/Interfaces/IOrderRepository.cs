namespace Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

using Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Repository interface for Order aggregate operations.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order if found; otherwise, null.</returns>
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all orders for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of orders for the user.</returns>
    Task<List<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all orders in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all orders.</returns>
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new order to the repository.
    /// </summary>
    /// <param name="order">The order to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="order">The order to update.</param>
    void Update(Order order);

    /// <summary>
    /// Deletes an order from the repository.
    /// </summary>
    /// <param name="order">The order to delete.</param>
    void Delete(Order order);
}
