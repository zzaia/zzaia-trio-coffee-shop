using MediatR;
using Zzaia.CoffeeShop.Order.Domain.Common;

namespace Zzaia.CoffeeShop.Order.Domain.Events;

/// <summary>
/// Domain event raised when an order is created.
/// </summary>
public sealed record OrderCreatedEvent(
    Guid OrderId,
    string UserId,
    decimal TotalAmount,
    string Currency,
    DateTimeOffset CreatedAt) : IDomainEvent, INotification;
