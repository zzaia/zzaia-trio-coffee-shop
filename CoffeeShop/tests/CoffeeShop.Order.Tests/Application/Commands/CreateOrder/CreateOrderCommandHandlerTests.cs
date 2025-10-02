namespace Zzaia.CoffeeShop.Order.Tests.Application.Commands.CreateOrder;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Commands.CreateOrder;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;
using OrderEntity = Zzaia.CoffeeShop.Order.Domain.Entities.Order;

/// <summary>
/// Unit tests for CreateOrderCommandHandler.
/// </summary>
public sealed class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> orderRepositoryMock;
    private readonly Mock<IProductRepository> productRepositoryMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<ILogger<CreateOrderCommandHandler>> loggerMock;
    private readonly CreateOrderCommandHandler handler;

    public CreateOrderCommandHandlerTests()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        productRepositoryMock = new Mock<IProductRepository>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<CreateOrderCommandHandler>>();
        handler = new CreateOrderCommandHandler(
            orderRepositoryMock.Object,
            productRepositoryMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrderSuccessfully_WhenValidRequest()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        Product product = Product.Create(
            "Espresso",
            "Strong coffee",
            Money.Create(10.00m),
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            [new OrderItemRequest(productId, null, 2)]);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        orderRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        CreateOrderCommand command = new(
            userId,
            [new OrderItemRequest(productId, null, 2)]);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Product with ID");
        result.Error.Should().Contain("not found");
        orderRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductIsNotAvailable()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        Product product = Product.Create(
            "Espresso",
            "Strong coffee",
            Money.Create(10.00m),
            "Coffee");
        product.SetAvailability(false);
        CreateOrderCommand command = new(
            userId,
            [new OrderItemRequest(productId, null, 2)]);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not available");
        orderRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenVariationNotFound()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        Guid variationId = Guid.NewGuid();
        Product product = Product.Create(
            "Latte",
            "Coffee with milk",
            Money.Create(12.00m),
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            [new OrderItemRequest(productId, variationId, 1)]);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        productRepositoryMock
            .Setup(x => x.GetVariationByIdAsync(variationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductVariation?)null);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Product variation with ID");
        result.Error.Should().Contain("not found");
        orderRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCalculateTotalWithVariation_WhenVariationProvided()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        Guid variationId = Guid.NewGuid();
        Product product = Product.Create(
            "Latte",
            "Coffee with milk",
            Money.Create(12.00m),
            "Coffee");
        ProductVariation variation = ProductVariation.Create(
            productId,
            "Large",
            Money.Create(3.00m));
        CreateOrderCommand command = new(
            userId,
            [new OrderItemRequest(productId, variationId, 2)]);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        productRepositoryMock
            .Setup(x => x.GetVariationByIdAsync(variationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(variation);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        orderRepositoryMock.Verify(
            x => x.AddAsync(It.Is<OrderEntity>(o => o.TotalAmount.Amount == 30.00m), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrderWithMultipleItems_WhenMultipleItemsProvided()
    {
        string userId = "user123";
        Guid productId1 = Guid.NewGuid();
        Guid productId2 = Guid.NewGuid();
        Product product1 = Product.Create(
            "Espresso",
            "Strong coffee",
            Money.Create(10.00m),
            "Coffee");
        Product product2 = Product.Create(
            "Cappuccino",
            "Coffee with foam",
            Money.Create(12.00m),
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            [
                new OrderItemRequest(productId1, null, 2),
                new OrderItemRequest(productId2, null, 1)
            ]);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product1);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product2);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        orderRepositoryMock.Verify(
            x => x.AddAsync(It.Is<OrderEntity>(o => o.Items.Count == 2 && o.TotalAmount.Amount == 32.00m), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        CreateOrderCommand command = new(
            userId,
            [new OrderItemRequest(productId, null, 2)]);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to create order");
        result.Error.Should().Contain("Database error");
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenOrderCreatedSuccessfully()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        Product product = Product.Create(
            "Espresso",
            "Strong coffee",
            Money.Create(10.00m),
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            [new OrderItemRequest(productId, null, 2)]);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("created successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionOccurs()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        CreateOrderCommand command = new(
            userId,
            [new OrderItemRequest(productId, null, 2)]);
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Error creating order")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
