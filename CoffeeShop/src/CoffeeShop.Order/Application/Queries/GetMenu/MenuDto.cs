namespace Zzaia.CoffeeShop.Order.Application.Queries.GetMenu;

/// <summary>
/// Represents the menu data transfer object.
/// </summary>
/// <param name="Products">The list of products in the menu.</param>
public record MenuDto(
    List<ProductDto> Products
);

/// <summary>
/// Represents a product data transfer object.
/// </summary>
/// <param name="ProductId">The product identifier.</param>
/// <param name="Name">The product name.</param>
/// <param name="Description">The product description.</param>
/// <param name="BasePrice">The base price amount.</param>
/// <param name="Currency">The currency code.</param>
/// <param name="Category">The product category.</param>
/// <param name="ImageUrl">The image URL.</param>
/// <param name="IsAvailable">The availability status.</param>
/// <param name="Variations">The product variations.</param>
public record ProductDto(
    Guid ProductId,
    string Name,
    string Description,
    decimal BasePrice,
    string Currency,
    string Category,
    string? ImageUrl,
    bool IsAvailable,
    List<ProductVariationDto> Variations
);

/// <summary>
/// Represents a product variation data transfer object.
/// </summary>
/// <param name="VariationId">The variation identifier.</param>
/// <param name="Name">The variation name.</param>
/// <param name="PriceAdjustment">The price adjustment amount.</param>
/// <param name="Currency">The currency code.</param>
public record ProductVariationDto(
    Guid VariationId,
    string Name,
    decimal PriceAdjustment,
    string Currency
);
