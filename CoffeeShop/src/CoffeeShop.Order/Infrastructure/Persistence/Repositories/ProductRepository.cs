namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Repository implementation for Product entity.
/// </summary>
internal sealed class ProductRepository(OrderDbContext context) : IProductRepository
{
    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found; otherwise, null.</returns>
    public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Variations)
            .FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
    }

    /// <summary>
    /// Retrieves all available products.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all available products.</returns>
    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Variations)
            .Where(p => p.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all available products by category.
    /// </summary>
    /// <param name="category">The product category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of products in the specified category.</returns>
    public async Task<List<Product>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Variations)
            .Where(p => p.Category == category && p.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a product variation by its unique identifier.
    /// </summary>
    /// <param name="variationId">The variation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product variation if found; otherwise, null.</returns>
    public async Task<ProductVariation?> GetVariationByIdAsync(Guid variationId, CancellationToken cancellationToken = default)
    {
        return await context.ProductVariations
            .FirstOrDefaultAsync(v => v.VariationId == variationId, cancellationToken);
    }
}
