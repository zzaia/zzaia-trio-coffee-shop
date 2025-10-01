using Zzaia.CoffeeShop.Order.Domain.Common;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Represents an item in an order.
/// </summary>
public sealed class OrderItem : Entity
{
    /// <summary>
    /// Gets the order item identifier.
    /// </summary>
    public Guid OrderItemId => Id;

    /// <summary>
    /// Gets the order identifier.
    /// </summary>
    public required Guid OrderId { get; init; }

    /// <summary>
    /// Gets the product snapshot.
    /// </summary>
    public required ProductSnapshot ProductSnapshot { get; init; }

    /// <summary>
    /// Gets the quantity.
    /// </summary>
    public required Quantity Quantity { get; init; }

    /// <summary>
    /// Gets the subtotal.
    /// </summary>
    public Money Subtotal { get; private set; }

    private OrderItem()
    {
        Subtotal = Money.Create(0);
    }

    /// <summary>
    /// Creates a new OrderItem instance.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="productSnapshot">The product snapshot.</param>
    /// <param name="quantity">The quantity.</param>
    /// <returns>A new OrderItem instance.</returns>
    public static OrderItem Create(Guid orderId, ProductSnapshot productSnapshot, Quantity quantity)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
        }
        OrderItem orderItem = new()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductSnapshot = productSnapshot ?? throw new ArgumentNullException(nameof(productSnapshot)),
            Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity))
        };
        orderItem.CalculateSubtotal();
        return orderItem;
    }

    /// <summary>
    /// Calculates the subtotal for this order item.
    /// </summary>
    public void CalculateSubtotal()
    {
        Subtotal = ProductSnapshot.UnitPrice * Quantity.Value;
    }
}
