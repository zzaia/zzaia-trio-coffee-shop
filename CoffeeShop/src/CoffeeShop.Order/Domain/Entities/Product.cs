using Zzaia.CoffeeShop.Order.Domain.Common;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Represents a product.
/// </summary>
public sealed class Product : Entity
{
    private readonly List<ProductVariation> variations = [];

    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid ProductId => Id;

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the product description.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the base price.
    /// </summary>
    public required Money BasePrice { get; init; }

    /// <summary>
    /// Gets the product category.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets the image URL.
    /// </summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    /// Gets the availability status.
    /// </summary>
    public bool IsAvailable { get; private set; }

    /// <summary>
    /// Gets the product variations.
    /// </summary>
    public IReadOnlyList<ProductVariation> Variations => variations.AsReadOnly();

    private Product()
    {
        IsAvailable = true;
    }

    /// <summary>
    /// Creates a new Product instance.
    /// </summary>
    /// <param name="name">The product name.</param>
    /// <param name="description">The product description.</param>
    /// <param name="basePrice">The base price.</param>
    /// <param name="category">The product category.</param>
    /// <param name="imageUrl">The image URL.</param>
    /// <returns>A new Product instance.</returns>
    public static Product Create(
        string name,
        string description,
        Money basePrice,
        string category,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be empty.", nameof(description));
        }
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category cannot be empty.", nameof(category));
        }
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            BasePrice = basePrice ?? throw new ArgumentNullException(nameof(basePrice)),
            Category = category,
            ImageUrl = imageUrl
        };
    }

    /// <summary>
    /// Adds a variation to the product.
    /// </summary>
    /// <param name="variation">The variation to add.</param>
    public void AddVariation(ProductVariation variation)
    {
        if (variation.ProductId != ProductId)
        {
            throw new InvalidOperationException("Variation does not belong to this product.");
        }
        variations.Add(variation);
    }

    /// <summary>
    /// Sets the product availability.
    /// </summary>
    /// <param name="isAvailable">The availability status.</param>
    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }
}
