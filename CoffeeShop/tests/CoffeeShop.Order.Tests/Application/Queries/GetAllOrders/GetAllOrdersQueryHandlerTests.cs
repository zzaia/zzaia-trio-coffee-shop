namespace Zzaia.CoffeeShop.Order.Tests.Application.Queries.GetAllOrders;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Application.Queries.GetAllOrders;
using Zzaia.CoffeeShop.Order.Application.Queries.GetOrderById;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;
using OrderEntity = Zzaia.CoffeeShop.Order.Domain.Entities.Order;

/// <summary>
/// Unit tests for GetAllOrdersQueryHandler.
/// </summary>
public sealed class GetAllOrdersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> orderRepositoryMock;
    private readonly Mock<ILogger<GetAllOrdersQueryHandler>> loggerMock;
    private readonly GetAllOrdersQueryHandler handler;

    public GetAllOrdersQueryHandlerTests()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        loggerMock = new Mock<ILogger<GetAllOrdersQueryHandler>>();
        handler = new GetAllOrdersQueryHandler(
            orderRepositoryMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllOrders_WhenUserIsManager()
    {
        string managerId = "manager123";
        OrderEntity order1 = OrderEntity.Create("user1");
        OrderEntity order2 = OrderEntity.Create("user2");
        ProductSnapshot product1 = ProductSnapshot.Create(
            Guid.NewGuid(),
            "Espresso",
            "Strong coffee",
            Money.Create(10.00m));
        order1.AddItem(product1, Quantity.Create(2));
        order2.AddItem(product1, Quantity.Create(1));
        List<OrderEntity> orders = [order1, order2];
        GetAllOrdersQuery query = new(managerId, true);
        orderRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        Result<List<OrderDto>> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(2);
        result.Value[0].UserId.Should().Be("user1");
        result.Value[1].UserId.Should().Be("user2");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotManager()
    {
        string userId = "user123";
        GetAllOrdersQuery query = new(userId, false);
        Result<List<OrderDto>> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not authorized");
        orderRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoOrdersExist()
    {
        string managerId = "manager123";
        List<OrderEntity> orders = [];
        GetAllOrdersQuery query = new(managerId, true);
        orderRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        Result<List<OrderDto>> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldIncludeAllOrderDetails_WhenOrdersHaveMultipleItems()
    {
        string managerId = "manager123";
        OrderEntity order = OrderEntity.Create("user1");
        ProductSnapshot product1 = ProductSnapshot.Create(
            Guid.NewGuid(),
            "Espresso",
            "Strong coffee",
            Money.Create(10.00m));
        ProductSnapshot product2 = ProductSnapshot.Create(
            Guid.NewGuid(),
            "Cappuccino",
            "Coffee with foam",
            Money.Create(12.00m));
        order.AddItem(product1, Quantity.Create(2));
        order.AddItem(product2, Quantity.Create(1));
        List<OrderEntity> orders = [order];
        GetAllOrdersQuery query = new(managerId, true);
        orderRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        Result<List<OrderDto>> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
        result.Value[0].Items.Should().HaveCount(2);
        result.Value[0].Items[0].ProductName.Should().Be("Espresso");
        result.Value[0].Items[1].ProductName.Should().Be("Cappuccino");
        result.Value[0].TotalAmount.Should().Be(32.00m);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
    {
        string managerId = "manager123";
        GetAllOrdersQuery query = new(managerId, true);
        orderRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result<List<OrderDto>> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to retrieve orders");
        result.Error.Should().Contain("Database error");
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenOrdersRetrievedSuccessfully()
    {
        string managerId = "manager123";
        OrderEntity order1 = OrderEntity.Create("user1");
        OrderEntity order2 = OrderEntity.Create("user2");
        List<OrderEntity> orders = [order1, order2];
        GetAllOrdersQuery query = new(managerId, true);
        orderRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        Result<List<OrderDto>> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Retrieved") && o.ToString()!.Contains("orders for manager")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogWarning_WhenNonManagerAttemptsAccess()
    {
        string userId = "user123";
        GetAllOrdersQuery query = new(userId, false);
        Result<List<OrderDto>> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Non-manager user") && o.ToString()!.Contains("attempted to access all orders")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionOccurs()
    {
        string managerId = "manager123";
        GetAllOrdersQuery query = new(managerId, true);
        orderRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result<List<OrderDto>> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Error retrieving all orders")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
