using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Common;
using Zzaia.CoffeeShop.Order.Domain.Events;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Events;

public sealed class OrderCreatedEventTests
{
    [Fact]
    public void Constructor_ShouldCreateEventWithValidParameters()
    {
        Guid orderId = Guid.NewGuid();
        string userId = "user123";
        Money totalAmount = Money.Create(50.00m);
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        OrderCreatedEvent domainEvent = new(orderId, userId, totalAmount, createdAt);
        domainEvent.OrderId.Should().Be(orderId);
        domainEvent.UserId.Should().Be(userId);
        domainEvent.TotalAmount.Should().Be(totalAmount);
        domainEvent.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void ShouldImplementIDomainEvent()
    {
        Guid orderId = Guid.NewGuid();
        string userId = "user123";
        Money totalAmount = Money.Create(50.00m);
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        OrderCreatedEvent domainEvent = new(orderId, userId, totalAmount, createdAt);
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }
}
