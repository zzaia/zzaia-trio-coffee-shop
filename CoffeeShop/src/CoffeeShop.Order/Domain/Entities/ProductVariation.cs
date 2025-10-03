using Zzaia.CoffeeShop.Order.Domain.Common;

namespace Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Represents a product variation.
/// </summary>
public sealed class ProductVariation : Entity
{
    /// <summary>
    /// Gets the variation identifier.
    /// </summary>
    public Guid VariationId => Id;

    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public required Guid ProductId { get; init; }

    /// <summary>
    /// Gets the variation name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the price adjustment amount.
    /// </summary>
    public required decimal PriceAdjustmentAmount { get; init; }

    /// <summary>
    /// Gets the currency.
    /// </summary>
    public required string Currency { get; init; }

    private ProductVariation()
    {
    }

    /// <summary>
    /// Creates a new ProductVariation instance.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="name">The variation name.</param>
    /// <param name="priceAdjustmentAmount">The price adjustment amount.</param>
    /// <param name="currency">The currency code.</param>
    /// <returns>A new ProductVariation instance.</returns>
    public static ProductVariation Create(Guid productId, string name, decimal priceAdjustmentAmount, string currency = "BRL")
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));
        }
        return new ProductVariation
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = name,
            PriceAdjustmentAmount = priceAdjustmentAmount,
            Currency = currency
        };
    }
}
