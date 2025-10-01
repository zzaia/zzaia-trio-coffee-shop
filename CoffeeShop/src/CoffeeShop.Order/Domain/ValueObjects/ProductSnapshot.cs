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
    /// Gets the unit price.
    /// </summary>
    public Money UnitPrice { get; init; }

    /// <summary>
    /// Gets the variation name.
    /// </summary>
    public string? VariationName { get; init; }

    private ProductSnapshot()
    {
        ProductId = Guid.Empty;
        Name = string.Empty;
        Description = string.Empty;
        UnitPrice = Money.Create(0);
    }

    private ProductSnapshot(Guid productId, string name, string description, Money unitPrice, string? variationName)
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
        ProductId = productId;
        Name = name;
        Description = description;
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        VariationName = variationName;
    }

    /// <summary>
    /// Creates a new ProductSnapshot instance.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="name">The product name.</param>
    /// <param name="description">The product description.</param>
    /// <param name="unitPrice">The unit price.</param>
    /// <param name="variationName">The variation name.</param>
    /// <returns>A new ProductSnapshot instance.</returns>
    public static ProductSnapshot Create(
        Guid productId,
        string name,
        string description,
        Money unitPrice,
        string? variationName = null)
    {
        return new ProductSnapshot(productId, name, description, unitPrice, variationName);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ProductId;
        yield return Name;
        yield return Description;
        yield return UnitPrice;
        yield return VariationName;
    }
}
