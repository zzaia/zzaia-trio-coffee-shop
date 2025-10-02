namespace Zzaia.CoffeeShop.Order.Tests.Application.Queries.GetOrderById;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Application.Queries.GetOrderById;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;
using OrderEntity = Zzaia.CoffeeShop.Order.Domain.Entities.Order;

/// <summary>
/// Unit tests for GetOrderByIdQueryHandler.
/// </summary>
public sealed class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderRepository> orderRepositoryMock;
    private readonly Mock<ILogger<GetOrderByIdQueryHandler>> loggerMock;
    private readonly GetOrderByIdQueryHandler handler;

    public GetOrderByIdQueryHandlerTests()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        loggerMock = new Mock<ILogger<GetOrderByIdQueryHandler>>();
        handler = new GetOrderByIdQueryHandler(
            orderRepositoryMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrder_WhenOrderExistsAndUserIsAuthorized()
    {
        string userId = "user123";
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create(userId);
        ProductSnapshot productSnapshot = ProductSnapshot.Create(
            Guid.NewGuid(),
            "Espresso",
            "Strong coffee",
            Money.Create(10.00m));
        order.AddItem(productSnapshot, Quantity.Create(2));
        GetOrderByIdQuery query = new(orderId, userId);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        Result<OrderDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.OrderId.Should().Be(order.OrderId);
        result.Value.UserId.Should().Be(userId);
        result.Value.Status.Should().Be("Waiting");
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].ProductName.Should().Be("Espresso");
        result.Value.Items[0].Quantity.Should().Be(2);
        result.Value.TotalAmount.Should().Be(20.00m);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenOrderNotFound()
    {
        string userId = "user123";
        Guid orderId = Guid.NewGuid();
        GetOrderByIdQuery query = new(orderId, userId);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderEntity?)null);
        Result<OrderDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Order with ID");
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotAuthorized()
    {
        string ownerId = "owner123";
        string requesterId = "requester456";
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create(ownerId);
        GetOrderByIdQuery query = new(orderId, requesterId);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        Result<OrderDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not authorized");
    }

    [Fact]
    public async Task Handle_ShouldIncludeMultipleItems_WhenOrderHasMultipleItems()
    {
        string userId = "user123";
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create(userId);
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
        GetOrderByIdQuery query = new(orderId, userId);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        Result<OrderDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.Items[0].ProductName.Should().Be("Espresso");
        result.Value.Items[0].Quantity.Should().Be(2);
        result.Value.Items[0].Subtotal.Should().Be(20.00m);
        result.Value.Items[1].ProductName.Should().Be("Cappuccino");
        result.Value.Items[1].Quantity.Should().Be(1);
        result.Value.Items[1].Subtotal.Should().Be(12.00m);
        result.Value.TotalAmount.Should().Be(32.00m);
    }

    [Fact]
    public async Task Handle_ShouldIncludeVariation_WhenItemHasVariation()
    {
        string userId = "user123";
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create(userId);
        ProductSnapshot productSnapshot = ProductSnapshot.Create(
            Guid.NewGuid(),
            "Latte",
            "Coffee with milk",
            Money.Create(15.00m),
            "Large");
        order.AddItem(productSnapshot, Quantity.Create(1));
        GetOrderByIdQuery query = new(orderId, userId);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        Result<OrderDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items[0].VariationName.Should().Be("Large");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
    {
        string userId = "user123";
        Guid orderId = Guid.NewGuid();
        GetOrderByIdQuery query = new(orderId, userId);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result<OrderDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to retrieve order");
        result.Error.Should().Contain("Database error");
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenOrderRetrievedSuccessfully()
    {
        string userId = "user123";
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create(userId);
        GetOrderByIdQuery query = new(orderId, userId);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        Result<OrderDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Retrieved order") && o.ToString()!.Contains("for user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogWarning_WhenUserIsNotAuthorized()
    {
        string ownerId = "owner123";
        string requesterId = "requester456";
        Guid orderId = Guid.NewGuid();
        OrderEntity order = OrderEntity.Create(ownerId);
        GetOrderByIdQuery query = new(orderId, requesterId);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        Result<OrderDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("attempted to access order")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionOccurs()
    {
        string userId = "user123";
        Guid orderId = Guid.NewGuid();
        GetOrderByIdQuery query = new(orderId, userId);
        orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result<OrderDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Error retrieving order")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
