namespace Zzaia.CoffeeShop.Order.Tests.Application.Commands.CreateOrder;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Commands.CreateOrder;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Presentation.Endpoints;
using OrderEntity = Zzaia.CoffeeShop.Order.Domain.Entities.Order;

/// <summary>
/// Unit tests for CreateOrderCommandHandler.
/// </summary>
public sealed class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> orderRepositoryMock;
    private readonly Mock<IProductRepository> productRepositoryMock;
    private readonly Mock<IPaymentService> paymentServiceMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<ILogger<CreateOrderCommandHandler>> loggerMock;
    private readonly CreateOrderCommandHandler handler;

    public CreateOrderCommandHandlerTests()
    {
        orderRepositoryMock = new Mock<IOrderRepository>();
        productRepositoryMock = new Mock<IProductRepository>();
        paymentServiceMock = new Mock<IPaymentService>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<CreateOrderCommandHandler>>();
        handler = new CreateOrderCommandHandler(
            orderRepositoryMock.Object,
            productRepositoryMock.Object,
            paymentServiceMock.Object,
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
            10.00m,
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            new CreateOrderItemRequest(productId, 2, null));
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        paymentServiceMock
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult(true, "txn-123", null));
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        paymentServiceMock.Verify(
            x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
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
            new CreateOrderItemRequest(productId, 2, null));
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
            10.00m,
            "Coffee");
        product.SetAvailability(false);
        CreateOrderCommand command = new(
            userId,
            new CreateOrderItemRequest(productId, 2, null));
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
            12.00m,
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            new CreateOrderItemRequest(productId, 1, variationId));
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
            12.00m,
            "Coffee");
        ProductVariation variation = ProductVariation.Create(
            productId,
            "Large",
            3.00m);
        CreateOrderCommand command = new(
            userId,
            new CreateOrderItemRequest(productId, 2, variationId));
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        productRepositoryMock
            .Setup(x => x.GetVariationByIdAsync(variationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(variation);
        paymentServiceMock
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult(true, "txn-456", null));
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        orderRepositoryMock.Verify(
            x => x.AddAsync(It.Is<OrderEntity>(o => o.TotalAmount == 30.00m), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrderWithMultipleItems_WhenMultipleItemsProvided()
    {
        string userId = "user123";
        Guid productId1 = Guid.NewGuid();
        Product product1 = Product.Create(
            "Espresso",
            "Strong coffee",
            10.00m,
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            new CreateOrderItemRequest(productId1, 2, null));
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product1);
        paymentServiceMock
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult(true, "txn-789", null));
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        orderRepositoryMock.Verify(
            x => x.AddAsync(It.Is<OrderEntity>(o => o.Items.Count == 1 && o.TotalAmount == 20.00m), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        CreateOrderCommand command = new(
            userId,
            new CreateOrderItemRequest(productId, 2, null));
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
            10.00m,
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            new CreateOrderItemRequest(productId, 2, null));
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        paymentServiceMock
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult(true, "txn-999", null));
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
            new CreateOrderItemRequest(productId, 2, null));
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

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPaymentFails()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        Product product = Product.Create(
            "Espresso",
            "Strong coffee",
            10.00m,
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            new CreateOrderItemRequest(productId, 2, null));
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        paymentServiceMock
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult(false, null, "Insufficient funds"));
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Insufficient funds");
        orderRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<OrderEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldRefundPayment_WhenOrderCreationFailsAfterPayment()
    {
        string userId = "user123";
        Guid productId = Guid.NewGuid();
        Product product = Product.Create(
            "Espresso",
            "Strong coffee",
            10.00m,
            "Coffee");
        CreateOrderCommand command = new(
            userId,
            new CreateOrderItemRequest(productId, 2, null));
        productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        paymentServiceMock
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult(true, "txn-refund-123", null));
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database save error"));
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Database save error");
        paymentServiceMock.Verify(
            x => x.RefundPaymentAsync(
                "txn-refund-123",
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
