using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Entities;

public sealed class OrderItemTests
{
    [Fact]
    public void Create_ShouldCreateOrderItemWithValidParameters()
    {
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Cappuccino",
            "Coffee with milk foam",
            15.00m);
        Quantity quantity = Quantity.Create(2);
        OrderItem orderItem = OrderItem.Create(orderId, snapshot, quantity);
        orderItem.OrderId.Should().Be(orderId);
        orderItem.ProductSnapshot.Should().Be(snapshot);
        orderItem.Quantity.Should().Be(quantity);
        orderItem.SubtotalAmount.Should().Be(30.00m);
        orderItem.Currency.Should().Be("BRL");
        orderItem.OrderItemId.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenOrderIdIsEmpty()
    {
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Cappuccino",
            "Coffee with milk foam",
            15.00m);
        Quantity quantity = Quantity.Create(2);
        Action act = () => OrderItem.Create(Guid.Empty, snapshot, quantity);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Order ID cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenProductSnapshotIsNull()
    {
        Guid orderId = Guid.NewGuid();
        Quantity quantity = Quantity.Create(2);
        Action act = () => OrderItem.Create(orderId, null!, quantity);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenQuantityIsNull()
    {
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Cappuccino",
            "Coffee with milk foam",
            15.00m);
        Action act = () => OrderItem.Create(orderId, snapshot, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalculateSubtotal_ShouldCalculateCorrectSubtotal()
    {
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Latte",
            "Coffee with steamed milk",
            12.50m);
        Quantity quantity = Quantity.Create(3);
        OrderItem orderItem = OrderItem.Create(orderId, snapshot, quantity);
        orderItem.SubtotalAmount.Should().Be(37.50m);
    }
}
