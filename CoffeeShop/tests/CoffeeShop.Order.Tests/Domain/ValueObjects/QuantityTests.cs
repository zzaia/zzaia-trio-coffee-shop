using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.ValueObjects;

public sealed class QuantityTests
{
    [Fact]
    public void Create_ShouldCreateQuantityWithValidValue()
    {
        Quantity quantity = Quantity.Create(5);
        quantity.Value.Should().Be(5);
    }

    [Fact]
    public void Create_ShouldThrowException_WhenValueIsZero()
    {
        Action act = () => Quantity.Create(0);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantity must be positive.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenValueIsNegative()
    {
        Action act = () => Quantity.Create(-5);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantity must be positive.*");
    }

    [Fact]
    public void Increase_ShouldIncreaseQuantityByAmount()
    {
        Quantity quantity = Quantity.Create(5);
        Quantity result = quantity.Increase(3);
        result.Value.Should().Be(8);
    }

    [Fact]
    public void Decrease_ShouldDecreaseQuantityByAmount()
    {
        Quantity quantity = Quantity.Create(10);
        Quantity result = quantity.Decrease(3);
        result.Value.Should().Be(7);
    }

    [Fact]
    public void Decrease_ShouldThrowException_WhenResultIsNotPositive()
    {
        Quantity quantity = Quantity.Create(5);
        Action act = () => quantity.Decrease(5);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantity must be positive.*");
    }

    [Fact]
    public void AdditionOperator_ShouldAddTwoQuantities()
    {
        Quantity quantity1 = Quantity.Create(5);
        Quantity quantity2 = Quantity.Create(3);
        Quantity result = quantity1 + quantity2;
        result.Value.Should().Be(8);
    }

    [Fact]
    public void SubtractionOperator_ShouldSubtractTwoQuantities()
    {
        Quantity quantity1 = Quantity.Create(10);
        Quantity quantity2 = Quantity.Create(3);
        Quantity result = quantity1 - quantity2;
        result.Value.Should().Be(7);
    }

    [Fact]
    public void AdditionOperatorWithInt_ShouldAddIntToQuantity()
    {
        Quantity quantity = Quantity.Create(5);
        Quantity result = quantity + 3;
        result.Value.Should().Be(8);
    }

    [Fact]
    public void SubtractionOperatorWithInt_ShouldSubtractIntFromQuantity()
    {
        Quantity quantity = Quantity.Create(10);
        Quantity result = quantity - 3;
        result.Value.Should().Be(7);
    }

    [Fact]
    public void ImplicitConversionToInt_ShouldReturnValue()
    {
        Quantity quantity = Quantity.Create(5);
        int value = quantity;
        value.Should().Be(5);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenQuantitiesAreEqual()
    {
        Quantity quantity1 = Quantity.Create(5);
        Quantity quantity2 = Quantity.Create(5);
        quantity1.Should().Be(quantity2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenQuantitiesAreDifferent()
    {
        Quantity quantity1 = Quantity.Create(5);
        Quantity quantity2 = Quantity.Create(10);
        quantity1.Should().NotBe(quantity2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEqualInstances()
    {
        Quantity quantity1 = Quantity.Create(5);
        Quantity quantity2 = Quantity.Create(5);
        quantity1.GetHashCode().Should().Be(quantity2.GetHashCode());
    }
}
