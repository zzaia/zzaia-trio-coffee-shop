using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Entities;

public sealed class ProductVariationTests
{
    [Fact]
    public void Create_ShouldCreateProductVariationWithValidParameters()
    {
        Guid productId = Guid.NewGuid();
        Money priceAdjustment = Money.Create(2.50m);
        ProductVariation variation = ProductVariation.Create(
            productId,
            "Extra Shot",
            priceAdjustment);
        variation.ProductId.Should().Be(productId);
        variation.Name.Should().Be("Extra Shot");
        variation.PriceAdjustmentAmount.Should().Be(2.50m);
        variation.Currency.Should().Be("BRL");
        variation.VariationId.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenProductIdIsEmpty()
    {
        Money priceAdjustment = Money.Create(2.50m);
        Action act = () => ProductVariation.Create(
            Guid.Empty,
            "Extra Shot",
            priceAdjustment);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Product ID cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenNameIsEmpty()
    {
        Guid productId = Guid.NewGuid();
        Money priceAdjustment = Money.Create(2.50m);
        Action act = () => ProductVariation.Create(
            productId,
            "",
            priceAdjustment);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenPriceAdjustmentIsNull()
    {
        Guid productId = Guid.NewGuid();
        Action act = () => ProductVariation.Create(
            productId,
            "Extra Shot",
            null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
