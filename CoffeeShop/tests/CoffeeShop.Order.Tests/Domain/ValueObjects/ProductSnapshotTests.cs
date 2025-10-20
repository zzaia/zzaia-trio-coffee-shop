using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.ValueObjects;

public sealed class ProductSnapshotTests
{
    [Fact]
    public void Create_ShouldCreateProductSnapshotWithValidParameters()
    {
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            15.50m);
        snapshot.ProductId.Should().Be(productId);
        snapshot.Name.Should().Be("Espresso");
        snapshot.Description.Should().Be("Strong coffee");
        snapshot.UnitPriceAmount.Should().Be(15.50m);
        snapshot.Currency.Should().Be("BRL");
        snapshot.VariationName.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldCreateProductSnapshotWithVariation()
    {
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            15.50m,
            "BRL",
            "Double Shot");
        snapshot.ProductId.Should().Be(productId);
        snapshot.Name.Should().Be("Espresso");
        snapshot.Description.Should().Be("Strong coffee");
        snapshot.UnitPriceAmount.Should().Be(15.50m);
        snapshot.Currency.Should().Be("BRL");
        snapshot.VariationName.Should().Be("Double Shot");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenProductIdIsEmpty()
    {
        Action act = () => ProductSnapshot.Create(
            Guid.Empty,
            "Espresso",
            "Strong coffee",
            15.50m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Product ID cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenNameIsEmpty()
    {
        Guid productId = Guid.NewGuid();
        Action act = () => ProductSnapshot.Create(
            productId,
            "",
            "Strong coffee",
            15.50m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenDescriptionIsEmpty()
    {
        Guid productId = Guid.NewGuid();
        Action act = () => ProductSnapshot.Create(
            productId,
            "Espresso",
            "",
            15.50m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Description cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenUnitPriceIsNegative()
    {
        Guid productId = Guid.NewGuid();
        Action act = () => ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            -1.00m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Unit price cannot be negative.*");
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenSnapshotsAreEqual()
    {
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot1 = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            15.50m,
            "BRL",
            "Double Shot");
        ProductSnapshot snapshot2 = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            15.50m,
            "BRL",
            "Double Shot");
        snapshot1.Should().Be(snapshot2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenProductIdsAreDifferent()
    {
        ProductSnapshot snapshot1 = ProductSnapshot.Create(
            Guid.NewGuid(),
            "Espresso",
            "Strong coffee",
            15.50m);
        ProductSnapshot snapshot2 = ProductSnapshot.Create(
            Guid.NewGuid(),
            "Espresso",
            "Strong coffee",
            15.50m);
        snapshot1.Should().NotBe(snapshot2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEqualInstances()
    {
        Guid productId = Guid.NewGuid();
        ProductSnapshot snapshot1 = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            15.50m);
        ProductSnapshot snapshot2 = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            15.50m);
        snapshot1.GetHashCode().Should().Be(snapshot2.GetHashCode());
    }
}
