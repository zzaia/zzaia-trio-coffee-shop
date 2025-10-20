namespace Zzaia.CoffeeShop.Order.Application.Queries.GetOrderById;

/// <summary>
/// Represents an order data transfer object.
/// </summary>
/// <param name="OrderId">The order identifier.</param>
/// <param name="UserId">The user identifier.</param>
/// <param name="Items">The order items.</param>
/// <param name="TotalAmount">The total amount.</param>
/// <param name="Currency">The currency code.</param>
/// <param name="Status">The order status.</param>
/// <param name="PaymentTransactionId">The payment transaction identifier.</param>
/// <param name="CreatedAt">The creation timestamp.</param>
/// <param name="UpdatedAt">The last update timestamp.</param>
public record OrderDto(
    Guid OrderId,
    string UserId,
    List<OrderItemDto> Items,
    decimal TotalAmount,
    string Currency,
    string Status,
    string? PaymentTransactionId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

/// <summary>
/// Represents an order item data transfer object.
/// </summary>
/// <param name="OrderItemId">The order item identifier.</param>
/// <param name="ProductId">The product identifier.</param>
/// <param name="ProductName">The product name.</param>
/// <param name="ProductDescription">The product description.</param>
/// <param name="UnitPrice">The unit price.</param>
/// <param name="Currency">The currency code.</param>
/// <param name="VariationName">The variation name.</param>
/// <param name="Quantity">The quantity.</param>
/// <param name="Subtotal">The subtotal amount.</param>
public record OrderItemDto(
    Guid OrderItemId,
    Guid ProductId,
    string ProductName,
    string ProductDescription,
    decimal UnitPrice,
    string Currency,
    string? VariationName,
    int Quantity,
    decimal Subtotal
);
