using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Entities;

public sealed class ProductTests
{
    [Fact]
    public void Create_ShouldCreateProductWithValidParameters()
    {
        Money basePrice = Money.Create(15.00m);
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            basePrice,
            "Hot Drinks");
        product.Name.Should().Be("Cappuccino");
        product.Description.Should().Be("Coffee with milk foam");
        product.BasePrice.Should().Be(basePrice);
        product.Category.Should().Be("Hot Drinks");
        product.IsAvailable.Should().BeTrue();
        product.ImageUrl.Should().BeNull();
        product.ProductId.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldCreateProductWithImageUrl()
    {
        Money basePrice = Money.Create(15.00m);
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            basePrice,
            "Hot Drinks",
            "https://example.com/cappuccino.jpg");
        product.ImageUrl.Should().Be("https://example.com/cappuccino.jpg");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenNameIsEmpty()
    {
        Money basePrice = Money.Create(15.00m);
        Action act = () => Product.Create(
            "",
            "Coffee with milk foam",
            basePrice,
            "Hot Drinks");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenDescriptionIsEmpty()
    {
        Money basePrice = Money.Create(15.00m);
        Action act = () => Product.Create(
            "Cappuccino",
            "",
            basePrice,
            "Hot Drinks");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Description cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenCategoryIsEmpty()
    {
        Money basePrice = Money.Create(15.00m);
        Action act = () => Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            basePrice,
            "");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Category cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenBasePriceIsNull()
    {
        Action act = () => Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            null!,
            "Hot Drinks");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddVariation_ShouldAddVariationToProduct()
    {
        Money basePrice = Money.Create(15.00m);
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            basePrice,
            "Hot Drinks");
        Money priceAdjustment = Money.Create(2.00m);
        ProductVariation variation = ProductVariation.Create(
            product.ProductId,
            "Extra Shot",
            priceAdjustment);
        product.AddVariation(variation);
        product.Variations.Should().HaveCount(1);
        product.Variations[0].Should().Be(variation);
    }

    [Fact]
    public void AddVariation_ShouldThrowException_WhenVariationDoesNotBelongToProduct()
    {
        Money basePrice = Money.Create(15.00m);
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            basePrice,
            "Hot Drinks");
        Money priceAdjustment = Money.Create(2.00m);
        ProductVariation variation = ProductVariation.Create(
            Guid.NewGuid(),
            "Extra Shot",
            priceAdjustment);
        Action act = () => product.AddVariation(variation);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Variation does not belong to this product.");
    }

    [Fact]
    public void SetAvailability_ShouldUpdateAvailabilityStatus()
    {
        Money basePrice = Money.Create(15.00m);
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            basePrice,
            "Hot Drinks");
        product.SetAvailability(false);
        product.IsAvailable.Should().BeFalse();
        product.SetAvailability(true);
        product.IsAvailable.Should().BeTrue();
    }
}
