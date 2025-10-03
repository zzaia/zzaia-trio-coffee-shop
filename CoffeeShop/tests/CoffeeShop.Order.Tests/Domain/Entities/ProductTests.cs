using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Entities;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Entities;

public sealed class ProductTests
{
    [Fact]
    public void Create_ShouldCreateProductWithValidParameters()
    {
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            15.00m,
            "Hot Drinks");
        product.Name.Should().Be("Cappuccino");
        product.Description.Should().Be("Coffee with milk foam");
        product.BasePriceAmount.Should().Be(15.00m);
        product.Currency.Should().Be("BRL");
        product.Category.Should().Be("Hot Drinks");
        product.IsAvailable.Should().BeTrue();
        product.ImageUrl.Should().BeNull();
        product.ProductId.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldCreateProductWithImageUrl()
    {
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            15.00m,
            "Hot Drinks",
            "BRL",
            "https://example.com/cappuccino.jpg");
        product.ImageUrl.Should().Be("https://example.com/cappuccino.jpg");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenNameIsEmpty()
    {
        Action act = () => Product.Create(
            "",
            "Coffee with milk foam",
            15.00m,
            "Hot Drinks");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenDescriptionIsEmpty()
    {
        Action act = () => Product.Create(
            "Cappuccino",
            "",
            15.00m,
            "Hot Drinks");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Description cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenCategoryIsEmpty()
    {
        Action act = () => Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            15.00m,
            "");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Category cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenBasePriceIsNegative()
    {
        Action act = () => Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            -1.00m,
            "Hot Drinks");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Base price cannot be negative.*");
    }

    [Fact]
    public void AddVariation_ShouldAddVariationToProduct()
    {
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            15.00m,
            "Hot Drinks");
        ProductVariation variation = ProductVariation.Create(
            product.ProductId,
            "Extra Shot",
            2.00m);
        product.AddVariation(variation);
        product.Variations.Should().HaveCount(1);
        product.Variations[0].Should().Be(variation);
    }

    [Fact]
    public void AddVariation_ShouldThrowException_WhenVariationDoesNotBelongToProduct()
    {
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            15.00m,
            "Hot Drinks");
        ProductVariation variation = ProductVariation.Create(
            Guid.NewGuid(),
            "Extra Shot",
            2.00m);
        Action act = () => product.AddVariation(variation);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Variation does not belong to this product.");
    }

    [Fact]
    public void SetAvailability_ShouldUpdateAvailabilityStatus()
    {
        Product product = Product.Create(
            "Cappuccino",
            "Coffee with milk foam",
            15.00m,
            "Hot Drinks");
        product.SetAvailability(false);
        product.IsAvailable.Should().BeFalse();
        product.SetAvailability(true);
        product.IsAvailable.Should().BeTrue();
    }
}
