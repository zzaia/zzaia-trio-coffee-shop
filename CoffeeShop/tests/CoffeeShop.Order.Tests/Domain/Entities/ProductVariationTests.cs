using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Entities;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Entities;

public sealed class ProductVariationTests
{
    [Fact]
    public void Create_ShouldCreateProductVariationWithValidParameters()
    {
        Guid productId = Guid.NewGuid();
        ProductVariation variation = ProductVariation.Create(
            productId,
            "Extra Shot",
            2.50m);
        variation.ProductId.Should().Be(productId);
        variation.Name.Should().Be("Extra Shot");
        variation.PriceAdjustmentAmount.Should().Be(2.50m);
        variation.Currency.Should().Be("BRL");
        variation.VariationId.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenProductIdIsEmpty()
    {
        Action act = () => ProductVariation.Create(
            Guid.Empty,
            "Extra Shot",
            2.50m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Product ID cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenNameIsEmpty()
    {
        Guid productId = Guid.NewGuid();
        Action act = () => ProductVariation.Create(
            productId,
            "",
            2.50m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenPriceAdjustmentIsNegative()
    {
        Guid productId = Guid.NewGuid();
        Action act = () => ProductVariation.Create(
            productId,
            "Extra Shot",
            -1.00m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Price adjustment cannot be negative.*");
    }
}
