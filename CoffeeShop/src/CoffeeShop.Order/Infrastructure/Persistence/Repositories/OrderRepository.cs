namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Repository implementation for Order aggregate.
/// </summary>
internal sealed class OrderRepository(OrderDbContext context) : IOrderRepository
{
    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order if found; otherwise, null.</returns>
    public async Task<Domain.Entities.Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    /// <summary>
    /// Retrieves all orders for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of orders for the user.</returns>
    public async Task<List<Domain.Entities.Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all orders in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all orders.</returns>
    public async Task<List<Domain.Entities.Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new order to the repository.
    /// </summary>
    /// <param name="order">The order to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task AddAsync(Domain.Entities.Order order, CancellationToken cancellationToken = default)
    {
        await context.Orders.AddAsync(order, cancellationToken);
    }

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="order">The order to update.</param>
    public void Update(Domain.Entities.Order order)
    {
        context.Orders.Update(order);
    }

    /// <summary>
    /// Deletes an order from the repository.
    /// </summary>
    /// <param name="order">The order to delete.</param>
    public void Delete(Domain.Entities.Order order)
    {
        context.Orders.Remove(order);
    }
}
