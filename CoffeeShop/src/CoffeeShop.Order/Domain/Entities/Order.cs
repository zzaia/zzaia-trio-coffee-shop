using Zzaia.CoffeeShop.Order.Domain.Common;
using Zzaia.CoffeeShop.Order.Domain.Enums;
using Zzaia.CoffeeShop.Order.Domain.Events;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Represents an order aggregate root.
/// </summary>
public sealed class Order : Entity
{
    private readonly List<OrderItem> items = [];

    /// <summary>
    /// Gets the order identifier.
    /// </summary>
    public Guid OrderId => Id;

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Gets the order items.
    /// </summary>
    public IReadOnlyList<OrderItem> Items => items.AsReadOnly();

    /// <summary>
    /// Gets the total amount.
    /// </summary>
    public Money TotalAmount { get; private set; }

    /// <summary>
    /// Gets the order status.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Gets the payment transaction identifier.
    /// </summary>
    public string? PaymentTransactionId { get; private set; }

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    private Order()
    {
        TotalAmount = Money.Create(0);
        Status = OrderStatus.Waiting;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Creates a new Order instance.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A new Order instance.</returns>
    public static Order Create(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }
        Order order = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };
        order.AddDomainEvent(new OrderCreatedEvent(
            order.OrderId,
            order.UserId,
            order.TotalAmount,
            order.CreatedAt));
        return order;
    }

    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    /// <param name="productSnapshot">The product snapshot.</param>
    /// <param name="quantity">The quantity.</param>
    public void AddItem(ProductSnapshot productSnapshot, Quantity quantity)
    {
        if (Status != OrderStatus.Waiting)
        {
            throw new InvalidOperationException("Cannot add items to an order that is not waiting.");
        }
        OrderItem orderItem = OrderItem.Create(OrderId, productSnapshot, quantity);
        items.Add(orderItem);
        CalculateTotal();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Removes an item from the order.
    /// </summary>
    /// <param name="orderItemId">The order item identifier.</param>
    public void RemoveItem(Guid orderItemId)
    {
        if (Status != OrderStatus.Waiting)
        {
            throw new InvalidOperationException("Cannot remove items from an order that is not waiting.");
        }
        OrderItem? item = items.FirstOrDefault(i => i.OrderItemId == orderItemId);
        if (item is null)
        {
            throw new InvalidOperationException("Order item not found.");
        }
        items.Remove(item);
        CalculateTotal();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Updates the order status.
    /// </summary>
    /// <param name="newStatus">The new status.</param>
    public void UpdateStatus(OrderStatus newStatus)
    {
        if (!IsValidStatusTransition(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid status transition from {Status} to {newStatus}.");
        }
        OrderStatus previousStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new OrderStatusChangedEvent(
            OrderId,
            previousStatus,
            newStatus,
            UpdatedAt));
    }

    /// <summary>
    /// Sets the payment transaction identifier.
    /// </summary>
    /// <param name="transactionId">The payment transaction identifier.</param>
    public void SetPaymentTransactionId(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            throw new ArgumentException("Transaction ID cannot be empty.", nameof(transactionId));
        }
        PaymentTransactionId = transactionId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Calculates the total amount of the order.
    /// </summary>
    public void CalculateTotal()
    {
        if (items.Count == 0)
        {
            TotalAmount = Money.Create(0);
            return;
        }
        Money total = items[0].Subtotal;
        for (int i = 1; i < items.Count; i++)
        {
            total = total.Add(items[i].Subtotal);
        }
        TotalAmount = total;
    }

    private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            (OrderStatus.Waiting, OrderStatus.Preparation) => true,
            (OrderStatus.Preparation, OrderStatus.Ready) => true,
            (OrderStatus.Ready, OrderStatus.Delivered) => true,
            _ => false
        };
    }
}
