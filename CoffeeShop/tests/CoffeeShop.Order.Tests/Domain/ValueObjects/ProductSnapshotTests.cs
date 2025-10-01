using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.ValueObjects;

public sealed class ProductSnapshotTests
{
    [Fact]
    public void Create_ShouldCreateProductSnapshotWithValidParameters()
    {
        Guid productId = Guid.NewGuid();
        Money unitPrice = Money.Create(15.50m);
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            unitPrice);
        snapshot.ProductId.Should().Be(productId);
        snapshot.Name.Should().Be("Espresso");
        snapshot.Description.Should().Be("Strong coffee");
        snapshot.UnitPrice.Should().Be(unitPrice);
        snapshot.VariationName.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldCreateProductSnapshotWithVariation()
    {
        Guid productId = Guid.NewGuid();
        Money unitPrice = Money.Create(15.50m);
        ProductSnapshot snapshot = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            unitPrice,
            "Double Shot");
        snapshot.ProductId.Should().Be(productId);
        snapshot.Name.Should().Be("Espresso");
        snapshot.Description.Should().Be("Strong coffee");
        snapshot.UnitPrice.Should().Be(unitPrice);
        snapshot.VariationName.Should().Be("Double Shot");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenProductIdIsEmpty()
    {
        Money unitPrice = Money.Create(15.50m);
        Action act = () => ProductSnapshot.Create(
            Guid.Empty,
            "Espresso",
            "Strong coffee",
            unitPrice);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Product ID cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenNameIsEmpty()
    {
        Guid productId = Guid.NewGuid();
        Money unitPrice = Money.Create(15.50m);
        Action act = () => ProductSnapshot.Create(
            productId,
            "",
            "Strong coffee",
            unitPrice);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenDescriptionIsEmpty()
    {
        Guid productId = Guid.NewGuid();
        Money unitPrice = Money.Create(15.50m);
        Action act = () => ProductSnapshot.Create(
            productId,
            "Espresso",
            "",
            unitPrice);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Description cannot be empty.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenUnitPriceIsNull()
    {
        Guid productId = Guid.NewGuid();
        Action act = () => ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenSnapshotsAreEqual()
    {
        Guid productId = Guid.NewGuid();
        Money unitPrice = Money.Create(15.50m);
        ProductSnapshot snapshot1 = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            unitPrice,
            "Double Shot");
        ProductSnapshot snapshot2 = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            unitPrice,
            "Double Shot");
        snapshot1.Should().Be(snapshot2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenProductIdsAreDifferent()
    {
        Money unitPrice = Money.Create(15.50m);
        ProductSnapshot snapshot1 = ProductSnapshot.Create(
            Guid.NewGuid(),
            "Espresso",
            "Strong coffee",
            unitPrice);
        ProductSnapshot snapshot2 = ProductSnapshot.Create(
            Guid.NewGuid(),
            "Espresso",
            "Strong coffee",
            unitPrice);
        snapshot1.Should().NotBe(snapshot2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEqualInstances()
    {
        Guid productId = Guid.NewGuid();
        Money unitPrice = Money.Create(15.50m);
        ProductSnapshot snapshot1 = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            unitPrice);
        ProductSnapshot snapshot2 = ProductSnapshot.Create(
            productId,
            "Espresso",
            "Strong coffee",
            unitPrice);
        snapshot1.GetHashCode().Should().Be(snapshot2.GetHashCode());
    }
}
