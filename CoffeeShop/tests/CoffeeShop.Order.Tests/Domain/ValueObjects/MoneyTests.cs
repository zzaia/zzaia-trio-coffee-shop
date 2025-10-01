using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.ValueObjects;

public sealed class MoneyTests
{
    [Fact]
    public void Create_ShouldCreateMoneyWithValidAmount()
    {
        Money money = Money.Create(100.50m);
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Create_ShouldCreateMoneyWithCustomCurrency()
    {
        Money money = Money.Create(100.50m, "USD");
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenAmountIsNegative()
    {
        Action act = () => Money.Create(-10m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Amount cannot be negative.*");
    }

    [Fact]
    public void Create_ShouldThrowException_WhenCurrencyIsEmpty()
    {
        Action act = () => Money.Create(100m, "");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency cannot be empty.*");
    }

    [Fact]
    public void Add_ShouldAddTwoMoneyInstances()
    {
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(50m);
        Money result = money1.Add(money2);
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Add_ShouldThrowException_WhenCurrenciesAreDifferent()
    {
        Money money1 = Money.Create(100m, "USD");
        Money money2 = Money.Create(50m, "BRL");
        Action act = () => money1.Add(money2);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add money with different currencies.");
    }

    [Fact]
    public void Subtract_ShouldSubtractTwoMoneyInstances()
    {
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(30m);
        Money result = money1.Subtract(money2);
        result.Amount.Should().Be(70m);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Subtract_ShouldThrowException_WhenCurrenciesAreDifferent()
    {
        Money money1 = Money.Create(100m, "USD");
        Money money2 = Money.Create(50m, "BRL");
        Action act = () => money1.Subtract(money2);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot subtract money with different currencies.");
    }

    [Fact]
    public void Multiply_ShouldMultiplyMoneyByFactor()
    {
        Money money = Money.Create(50m);
        Money result = money.Multiply(3m);
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void AdditionOperator_ShouldAddTwoMoneyInstances()
    {
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(50m);
        Money result = money1 + money2;
        result.Amount.Should().Be(150m);
    }

    [Fact]
    public void SubtractionOperator_ShouldSubtractTwoMoneyInstances()
    {
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(30m);
        Money result = money1 - money2;
        result.Amount.Should().Be(70m);
    }

    [Fact]
    public void MultiplicationOperator_ShouldMultiplyMoneyByFactor()
    {
        Money money = Money.Create(50m);
        Money result = money * 3m;
        result.Amount.Should().Be(150m);
    }

    [Fact]
    public void MultiplicationOperator_ShouldMultiplyFactorByMoney()
    {
        Money money = Money.Create(50m);
        Money result = 3m * money;
        result.Amount.Should().Be(150m);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenMoneyInstancesAreEqual()
    {
        Money money1 = Money.Create(100m, "USD");
        Money money2 = Money.Create(100m, "USD");
        money1.Should().Be(money2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenAmountsAreDifferent()
    {
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(50m);
        money1.Should().NotBe(money2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenCurrenciesAreDifferent()
    {
        Money money1 = Money.Create(100m, "USD");
        Money money2 = Money.Create(100m, "BRL");
        money1.Should().NotBe(money2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEqualInstances()
    {
        Money money1 = Money.Create(100m, "USD");
        Money money2 = Money.Create(100m, "USD");
        money1.GetHashCode().Should().Be(money2.GetHashCode());
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenMoneyInstancesAreEqual()
    {
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(100m);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_ShouldReturnTrue_WhenMoneyInstancesAreDifferent()
    {
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(50m);
        (money1 != money2).Should().BeTrue();
    }
}
