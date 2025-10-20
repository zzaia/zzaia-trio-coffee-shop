namespace Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

using Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Repository interface for Product entity operations.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found; otherwise, null.</returns>
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all available products.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all available products.</returns>
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all available products by category.
    /// </summary>
    /// <param name="category">The product category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of products in the specified category.</returns>
    Task<List<Product>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a product variation by its unique identifier.
    /// </summary>
    /// <param name="variationId">The variation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product variation if found; otherwise, null.</returns>
    Task<ProductVariation?> GetVariationByIdAsync(Guid variationId, CancellationToken cancellationToken = default);
}
