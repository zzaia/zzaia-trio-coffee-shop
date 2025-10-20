using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Enums;
using Zzaia.CoffeeShop.Order.Domain.Events;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;
using OrderEntity = Zzaia.CoffeeShop.Order.Domain.Entities.Order;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Entities;

public sealed class OrderTests
{
    [Fact]
    public void Create_ShouldCreateOrderWithValidUserId()
    {
        string userId = "user123";
        OrderEntity order = OrderEntity.Create(userId);
        order.UserId.Should().Be(userId);
        order.Status.Should().Be(OrderStatus.Waiting);
        order.TotalAmount.Should().Be(0m);
        order.Currency.Should().Be("BRL");
        order.Items.Should().BeEmpty();
        order.OrderId.Should().NotBeEmpty();
        order.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        order.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldRaiseOrderCreatedEvent()
    {
        string userId = "user123";
        OrderEntity order = OrderEntity.Create(userId);
        order.DomainEvents.Should().HaveCount(1);
        OrderCreatedEvent domainEvent = order.DomainEvents[0].Should().BeOfType<OrderCreatedEvent>().Subject;
        domainEvent.OrderId.Should().Be(order.OrderId);
        domainEvent.UserId.Should().Be(userId);
        domainEvent.TotalAmount.Should().Be(order.TotalAmount);
        domainEvent.Currency.Should().Be(order.Currency);
    }

    [Fact]
    public void Create_ShouldThrowException_WhenUserIdIsEmpty()
    {
        Action act = () => OrderEntity.Create("");
        act.Should().Throw<ArgumentException>()
            .WithMessage("User ID cannot be empty.*");
    }

    [Fact]
    public void AddItem_ShouldAddItemToOrder()
    {
        OrderEntity order = OrderEntity.Create("user123");
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Cappuccino",
            "Coffee with milk foam",
            15.00m);
        Quantity quantity = Quantity.Create(2);
        order.AddItem(snapshot, quantity);
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(30.00m);
    }

    [Fact]
    public void AddItem_ShouldUpdateTotalAmount()
    {
        OrderEntity order = OrderEntity.Create("user123");
        Guid productId1 = Guid.NewGuid();
        ProductSnapshot snapshot1 = ProductSnapshot.Create(
            productId1,
            "Cappuccino",
            "Coffee with milk foam",
            15.00m);
        Quantity quantity1 = Quantity.Create(2);
        order.AddItem(snapshot1, quantity1);
        Guid productId2 = Guid.NewGuid();
        ProductSnapshot snapshot2 = ProductSnapshot.Create(
            productId2,
            "Latte",
            "Coffee with steamed milk",
            10.00m);
        Quantity quantity2 = Quantity.Create(1);
        order.AddItem(snapshot2, quantity2);
        order.Items.Should().HaveCount(2);
        order.TotalAmount.Should().Be(40.00m);
    }

    [Fact]
    public void AddItem_ShouldThrowException_WhenStatusIsNotWaiting()
    {
        OrderEntity order = OrderEntity.Create("user123");
        order.UpdateStatus(OrderStatus.Preparation);
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Cappuccino",
            "Coffee with milk foam",
            15.00m);
        Quantity quantity = Quantity.Create(2);
        Action act = () => order.AddItem(snapshot, quantity);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add items to an order that is not waiting.");
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItemFromOrder()
    {
        OrderEntity order = OrderEntity.Create("user123");
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Cappuccino",
            "Coffee with milk foam",
            15.00m);
        Quantity quantity = Quantity.Create(2);
        order.AddItem(snapshot, quantity);
        Guid orderItemId = order.Items[0].OrderItemId;
        order.RemoveItem(orderItemId);
        order.Items.Should().BeEmpty();
        order.TotalAmount.Should().Be(0m);
    }

    [Fact]
    public void RemoveItem_ShouldThrowException_WhenItemNotFound()
    {
        OrderEntity order = OrderEntity.Create("user123");
        Action act = () => order.RemoveItem(Guid.NewGuid());
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Order item not found.");
    }

    [Fact]
    public void RemoveItem_ShouldThrowException_WhenStatusIsNotWaiting()
    {
        OrderEntity order = OrderEntity.Create("user123");
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Cappuccino",
            "Coffee with milk foam",
            15.00m);
        Quantity quantity = Quantity.Create(2);
        order.AddItem(snapshot, quantity);
        Guid orderItemId = order.Items[0].OrderItemId;
        order.UpdateStatus(OrderStatus.Preparation);
        Action act = () => order.RemoveItem(orderItemId);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot remove items from an order that is not waiting.");
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatusFromWaitingToPreparation()
    {
        OrderEntity order = OrderEntity.Create("user123");
        order.UpdateStatus(OrderStatus.Preparation);
        order.Status.Should().Be(OrderStatus.Preparation);
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatusFromPreparationToReady()
    {
        OrderEntity order = OrderEntity.Create("user123");
        order.UpdateStatus(OrderStatus.Preparation);
        order.UpdateStatus(OrderStatus.Ready);
        order.Status.Should().Be(OrderStatus.Ready);
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatusFromReadyToDelivered()
    {
        OrderEntity order = OrderEntity.Create("user123");
        order.UpdateStatus(OrderStatus.Preparation);
        order.UpdateStatus(OrderStatus.Ready);
        order.UpdateStatus(OrderStatus.Delivered);
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void UpdateStatus_ShouldRaiseOrderStatusChangedEvent()
    {
        OrderEntity order = OrderEntity.Create("user123");
        order.ClearDomainEvents();
        order.UpdateStatus(OrderStatus.Preparation);
        order.DomainEvents.Should().HaveCount(1);
        OrderStatusChangedEvent domainEvent = order.DomainEvents[0].Should().BeOfType<OrderStatusChangedEvent>().Subject;
        domainEvent.OrderId.Should().Be(order.OrderId);
        domainEvent.PreviousStatus.Should().Be(OrderStatus.Waiting);
        domainEvent.NewStatus.Should().Be(OrderStatus.Preparation);
    }

    [Fact]
    public void UpdateStatus_ShouldThrowException_WhenTransitionIsInvalid()
    {
        OrderEntity order = OrderEntity.Create("user123");
        Action act = () => order.UpdateStatus(OrderStatus.Ready);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Invalid status transition from Waiting to Ready.");
    }

    [Fact]
    public void SetPaymentTransactionId_ShouldSetTransactionId()
    {
        OrderEntity order = OrderEntity.Create("user123");
        order.SetPaymentTransactionId("txn123");
        order.PaymentTransactionId.Should().Be("txn123");
    }

    [Fact]
    public void SetPaymentTransactionId_ShouldThrowException_WhenTransactionIdIsEmpty()
    {
        OrderEntity order = OrderEntity.Create("user123");
        Action act = () => order.SetPaymentTransactionId("");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Transaction ID cannot be empty.*");
    }

    [Fact]
    public void CalculateTotal_ShouldCalculateTotalAmountCorrectly()
    {
        OrderEntity order = OrderEntity.Create("user123");
        Guid productId1 = Guid.NewGuid();
        ProductSnapshot snapshot1 = ProductSnapshot.Create(
            productId1,
            "Latte",
            "Coffee with steamed milk",
            12.50m);
        Quantity quantity1 = Quantity.Create(2);
        order.AddItem(snapshot1, quantity1);
        Guid productId2 = Guid.NewGuid();
        ProductSnapshot snapshot2 = ProductSnapshot.Create(
            productId2,
            "Cappuccino",
            "Coffee with milk foam",
            15.00m);
        Quantity quantity2 = Quantity.Create(3);
        order.AddItem(snapshot2, quantity2);
        order.TotalAmount.Should().Be(70.00m);
    }

    [Fact]
    public void CalculateTotal_ShouldReturnZero_WhenNoItems()
    {
        OrderEntity order = OrderEntity.Create("user123");
        order.TotalAmount.Should().Be(0m);
    }
}
