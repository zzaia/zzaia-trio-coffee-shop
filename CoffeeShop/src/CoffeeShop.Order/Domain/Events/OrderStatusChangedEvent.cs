using MediatR;
using Zzaia.CoffeeShop.Order.Domain.Common;
using Zzaia.CoffeeShop.Order.Domain.Enums;

namespace Zzaia.CoffeeShop.Order.Domain.Events;

/// <summary>
/// Domain event raised when an order status changes.
/// </summary>
public sealed record OrderStatusChangedEvent(
    Guid OrderId,
    OrderStatus PreviousStatus,
    OrderStatus NewStatus,
    DateTimeOffset ChangedAt) : IDomainEvent, INotification;
