using MediatR;
using Zzaia.CoffeeShop.Order.Domain.Common;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Domain.Events;

/// <summary>
/// Domain event raised when an order is created.
/// </summary>
public sealed record OrderCreatedEvent(
    Guid OrderId,
    string UserId,
    Money TotalAmount,
    DateTimeOffset CreatedAt) : IDomainEvent, INotification;
