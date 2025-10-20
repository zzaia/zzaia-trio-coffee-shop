using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Common;
using Zzaia.CoffeeShop.Order.Domain.Events;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Events;

public sealed class OrderCreatedEventTests
{
    [Fact]
    public void Constructor_ShouldCreateEventWithValidParameters()
    {
        Guid orderId = Guid.NewGuid();
        string userId = "user123";
        decimal totalAmount = 50.00m;
        string currency = "BRL";
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        OrderCreatedEvent domainEvent = new(orderId, userId, totalAmount, currency, createdAt);
        domainEvent.OrderId.Should().Be(orderId);
        domainEvent.UserId.Should().Be(userId);
        domainEvent.TotalAmount.Should().Be(totalAmount);
        domainEvent.Currency.Should().Be(currency);
        domainEvent.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void ShouldImplementIDomainEvent()
    {
        Guid orderId = Guid.NewGuid();
        string userId = "user123";
        decimal totalAmount = 50.00m;
        string currency = "BRL";
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        OrderCreatedEvent domainEvent = new(orderId, userId, totalAmount, currency, createdAt);
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }
}
