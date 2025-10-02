namespace Zzaia.CoffeeShop.Order.Tests.Application.Commands.UpdateOrderStatus;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Commands.UpdateOrderStatus;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Domain.Enums;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;
using OrderEntity = Zzaia.CoffeeShop.Order.Domain.Entities.Order;

/// <summary>
/// Unit tests for UpdateOrderStatusCommandHandler.
/// </summary>
public sealed class UpdateOrderStatusCommandHandlerTests
{
    private readonly Mock<IOrderRepository> orderRepositoryMock;
    private readonly Mock<INotificationService> notificationServiceMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<ILogger<UpdateOrderStatusCommandHandler>> loggerMock;
    private readonly UpdateOrderStatusCommandHandler handler;

    public UpdateOrderStatusCommandHandlerTests()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        notificationServiceMock = new Mock<INotificationService>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<UpdateOrderStatusCommandHandler>>();
        handler = new UpdateOrderStatusCommandHandler(
            orderRepositoryMock.Object,
            notificationServiceMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatusSuccessfully_WhenTransitionFromWaitingToPreparation()
    {
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create("user123");
        order.AddItem(
            ProductSnapshot.Create(
                Guid.NewGuid(),
                "Espresso",
                "Strong coffee",
                Money.Create(10.00m),
                null),
            Quantity.Create(1));
        UpdateOrderStatusCommand command = new(orderId, OrderStatus.Preparation);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        notificationServiceMock
            .Setup(x => x.SendOrderStatusNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Preparation);
        notificationServiceMock.Verify(
            x => x.SendOrderStatusNotificationAsync(
                order.UserId,
                order.OrderId,
                OrderStatus.Preparation.ToString(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        orderRepositoryMock.Verify(
            x => x.Update(It.IsAny<OrderEntity>()),
            Times.Once);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatusSuccessfully_WhenTransitionFromPreparationToReady()
    {
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create("user123");
        order.AddItem(
            ProductSnapshot.Create(
                Guid.NewGuid(),
                "Espresso",
                "Strong coffee",
                Money.Create(10.00m),
                null),
            Quantity.Create(1));
        order.UpdateStatus(OrderStatus.Preparation);
        UpdateOrderStatusCommand command = new(orderId, OrderStatus.Ready);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        notificationServiceMock
            .Setup(x => x.SendOrderStatusNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Ready);
        notificationServiceMock.Verify(
            x => x.SendOrderStatusNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                OrderStatus.Ready.ToString(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        orderRepositoryMock.Verify(
            x => x.Update(It.IsAny<OrderEntity>()),
            Times.Once);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatusSuccessfully_WhenTransitionFromReadyToDelivered()
    {
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create("user123");
        order.AddItem(
            ProductSnapshot.Create(
                Guid.NewGuid(),
                "Espresso",
                "Strong coffee",
                Money.Create(10.00m),
                null),
            Quantity.Create(1));
        order.UpdateStatus(OrderStatus.Preparation);
        order.UpdateStatus(OrderStatus.Ready);
        UpdateOrderStatusCommand command = new(orderId, OrderStatus.Delivered);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        notificationServiceMock
            .Setup(x => x.SendOrderStatusNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Delivered);
        notificationServiceMock.Verify(
            x => x.SendOrderStatusNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                OrderStatus.Delivered.ToString(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        orderRepositoryMock.Verify(
            x => x.Update(It.IsAny<OrderEntity>()),
            Times.Once);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenInvalidStatusTransition()
    {
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create("user123");
        order.AddItem(
            ProductSnapshot.Create(
                Guid.NewGuid(),
                "Espresso",
                "Strong coffee",
                Money.Create(10.00m),
                null),
            Quantity.Create(1));
        UpdateOrderStatusCommand command = new(orderId, OrderStatus.Delivered);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        Result result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid status transition");
        order.Status.Should().Be(OrderStatus.Waiting);
        orderRepositoryMock.Verify(
            x => x.Update(It.IsAny<OrderEntity>()),
            Times.Never);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenOrderNotFound()
    {
        Guid orderId = Guid.NewGuid();
        UpdateOrderStatusCommand command = new(orderId, OrderStatus.Preparation);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderEntity?)null);
        Result result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Order with ID");
        result.Error.Should().Contain("not found");
        orderRepositoryMock.Verify(
            x => x.Update(It.IsAny<OrderEntity>()),
            Times.Never);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
    {
        Guid orderId = Guid.NewGuid();
        UpdateOrderStatusCommand command = new(orderId, OrderStatus.Preparation);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to update order status");
        result.Error.Should().Contain("Database error");
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenStatusUpdatedSuccessfully()
    {
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create("user123");
        order.AddItem(
            ProductSnapshot.Create(
                Guid.NewGuid(),
                "Espresso",
                "Strong coffee",
                Money.Create(10.00m),
                null),
            Quantity.Create(1));
        UpdateOrderStatusCommand command = new(orderId, OrderStatus.Preparation);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        notificationServiceMock
            .Setup(x => x.SendOrderStatusNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("status updated")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionOccurs()
    {
        Guid orderId = Guid.NewGuid();
        UpdateOrderStatusCommand command = new(orderId, OrderStatus.Preparation);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Error updating order")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogWarning_WhenNotificationFails()
    {
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create("user123");
        order.AddItem(
            ProductSnapshot.Create(
                Guid.NewGuid(),
                "Espresso",
                "Strong coffee",
                Money.Create(10.00m),
                null),
            Quantity.Create(1));
        UpdateOrderStatusCommand command = new(orderId, OrderStatus.Preparation);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        notificationServiceMock
            .Setup(x => x.SendOrderStatusNotificationAsync(
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Failed to send notification")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
