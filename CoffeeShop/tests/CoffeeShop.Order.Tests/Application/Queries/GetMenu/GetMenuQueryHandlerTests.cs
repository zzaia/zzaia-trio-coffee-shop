namespace Zzaia.CoffeeShop.Order.Tests.Application.Queries.GetMenu;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Application.Queries.GetMenu;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

/// <summary>
/// Unit tests for GetMenuQueryHandler.
/// </summary>
public sealed class GetMenuQueryHandlerTests
{
    private readonly Mock<IProductRepository> productRepositoryMock;
    private readonly Mock<ILogger<GetMenuQueryHandler>> loggerMock;
    private readonly GetMenuQueryHandler handler;

    public GetMenuQueryHandlerTests()
    {
        productRepositoryMock = new Mock<IProductRepository>();
        loggerMock = new Mock<ILogger<GetMenuQueryHandler>>();
        handler = new GetMenuQueryHandler(
            productRepositoryMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMenuSuccessfully_WhenProductsExist()
    {
        Guid productId = Guid.NewGuid();
        Product product = Product.Create(
            "Espresso",
            "Strong coffee",
            Money.Create(10.00m),
            "Coffee");
        List<Product> products = [product];
        GetMenuQuery query = new();
        productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        Result<MenuDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Products.Should().HaveCount(1);
        result.Value.Products[0].ProductId.Should().Be(product.ProductId);
        result.Value.Products[0].Name.Should().Be("Espresso");
        result.Value.Products[0].Description.Should().Be("Strong coffee");
        result.Value.Products[0].BasePrice.Should().Be(10.00m);
        result.Value.Products[0].Currency.Should().Be("BRL");
        result.Value.Products[0].Category.Should().Be("Coffee");
        result.Value.Products[0].IsAvailable.Should().BeTrue();
        result.Value.Products[0].Variations.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyMenu_WhenNoProductsExist()
    {
        List<Product> products = [];
        GetMenuQuery query = new();
        productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        Result<MenuDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldIncludeVariations_WhenProductHasVariations()
    {
        Guid productId = Guid.NewGuid();
        Product product = Product.Create(
            "Latte",
            "Coffee with milk",
            Money.Create(12.00m),
            "Coffee");
        ProductVariation variation1 = ProductVariation.Create(
            product.ProductId,
            "Small",
            Money.Create(0.00m));
        ProductVariation variation2 = ProductVariation.Create(
            product.ProductId,
            "Large",
            Money.Create(3.00m));
        product.AddVariation(variation1);
        product.AddVariation(variation2);
        List<Product> products = [product];
        GetMenuQuery query = new();
        productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        Result<MenuDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Products.Should().HaveCount(1);
        result.Value.Products[0].Variations.Should().HaveCount(2);
        result.Value.Products[0].Variations[0].Name.Should().Be("Small");
        result.Value.Products[0].Variations[0].PriceAdjustment.Should().Be(0.00m);
        result.Value.Products[0].Variations[1].Name.Should().Be("Large");
        result.Value.Products[0].Variations[1].PriceAdjustment.Should().Be(3.00m);
    }

    [Fact]
    public async Task Handle_ShouldIncludeUnavailableProducts_WhenProductsAreNotAvailable()
    {
        Product product = Product.Create(
            "Espresso",
            "Strong coffee",
            Money.Create(10.00m),
            "Coffee");
        product.SetAvailability(false);
        List<Product> products = [product];
        GetMenuQuery query = new();
        productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        Result<MenuDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Products.Should().HaveCount(1);
        result.Value.Products[0].IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnMultipleProducts_WhenMultipleProductsExist()
    {
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
        Product product3 = Product.Create(
            "Croissant",
            "French pastry",
            Money.Create(8.00m),
            "Bakery");
        List<Product> products = [product1, product2, product3];
        GetMenuQuery query = new();
        productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        Result<MenuDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Products.Should().HaveCount(3);
        result.Value.Products[0].Name.Should().Be("Espresso");
        result.Value.Products[1].Name.Should().Be("Cappuccino");
        result.Value.Products[2].Name.Should().Be("Croissant");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
    {
        GetMenuQuery query = new();
        productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result<MenuDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to retrieve menu");
        result.Error.Should().Contain("Database error");
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenMenuRetrievedSuccessfully()
    {
        List<Product> products = [
            Product.Create("Espresso", "Strong coffee", Money.Create(10.00m), "Coffee"),
            Product.Create("Cappuccino", "Coffee with foam", Money.Create(12.00m), "Coffee")
        ];
        GetMenuQuery query = new();
        productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        Result<MenuDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Retrieved menu with") && o.ToString()!.Contains("products")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionOccurs()
    {
        GetMenuQuery query = new();
        productRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        Result<MenuDto> result = await handler.Handle(query, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Error retrieving menu")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
