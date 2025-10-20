using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Common;
using Zzaia.CoffeeShop.Order.Domain.Enums;
using Zzaia.CoffeeShop.Order.Domain.Events;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Events;

public sealed class OrderStatusChangedEventTests
{
    [Fact]
    public void Constructor_ShouldCreateEventWithValidParameters()
    {
        Guid orderId = Guid.NewGuid();
        OrderStatus previousStatus = OrderStatus.Waiting;
        OrderStatus newStatus = OrderStatus.Preparation;
        DateTimeOffset changedAt = DateTimeOffset.UtcNow;
        OrderStatusChangedEvent domainEvent = new(orderId, previousStatus, newStatus, changedAt);
        domainEvent.OrderId.Should().Be(orderId);
        domainEvent.PreviousStatus.Should().Be(previousStatus);
        domainEvent.NewStatus.Should().Be(newStatus);
        domainEvent.ChangedAt.Should().Be(changedAt);
    }

    [Fact]
    public void ShouldImplementIDomainEvent()
    {
        Guid orderId = Guid.NewGuid();
        OrderStatus previousStatus = OrderStatus.Waiting;
        OrderStatus newStatus = OrderStatus.Preparation;
        DateTimeOffset changedAt = DateTimeOffset.UtcNow;
        OrderStatusChangedEvent domainEvent = new(orderId, previousStatus, newStatus, changedAt);
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }
}
