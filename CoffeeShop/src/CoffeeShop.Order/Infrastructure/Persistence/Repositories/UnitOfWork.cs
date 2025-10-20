namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Repositories;

using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

/// <summary>
/// Unit of Work implementation for managing database transactions.
/// </summary>
internal sealed class UnitOfWork(OrderDbContext context) : IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
