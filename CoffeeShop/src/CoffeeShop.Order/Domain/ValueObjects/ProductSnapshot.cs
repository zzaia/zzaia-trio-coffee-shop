using Zzaia.CoffeeShop.Order.Domain.Common;

namespace Zzaia.CoffeeShop.Order.Domain.ValueObjects;

/// <summary>
/// Represents a snapshot of product information at the time of order.
/// </summary>
public sealed class ProductSnapshot : ValueObject
{
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the product description.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Gets the unit price amount.
    /// </summary>
    public decimal UnitPriceAmount { get; init; }

    /// <summary>
    /// Gets the currency.
    /// </summary>
    public string Currency { get; init; }

    /// <summary>
    /// Gets the variation name.
    /// </summary>
    public string? VariationName { get; init; }

    private ProductSnapshot()
    {
        ProductId = Guid.Empty;
        Name = string.Empty;
        Description = string.Empty;
        UnitPriceAmount = 0m;
        Currency = "BRL";
    }

    private ProductSnapshot(Guid productId, string name, string description, decimal unitPriceAmount, string currency, string? variationName)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be empty.", nameof(description));
        }
        if (unitPriceAmount < 0)
        {
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPriceAmount));
        }
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));
        }
        ProductId = productId;
        Name = name;
        Description = description;
        UnitPriceAmount = unitPriceAmount;
        Currency = currency;
        VariationName = variationName;
    }

    /// <summary>
    /// Creates a new ProductSnapshot instance.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="name">The product name.</param>
    /// <param name="description">The product description.</param>
    /// <param name="unitPriceAmount">The unit price amount.</param>
    /// <param name="currency">The currency.</param>
    /// <param name="variationName">The variation name.</param>
    /// <returns>A new ProductSnapshot instance.</returns>
    public static ProductSnapshot Create(
        Guid productId,
        string name,
        string description,
        decimal unitPriceAmount,
        string currency = "BRL",
        string? variationName = null)
    {
        return new ProductSnapshot(productId, name, description, unitPriceAmount, currency, variationName);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ProductId;
        yield return Name;
        yield return Description;
        yield return UnitPriceAmount;
        yield return Currency;
        yield return VariationName;
    }
}
