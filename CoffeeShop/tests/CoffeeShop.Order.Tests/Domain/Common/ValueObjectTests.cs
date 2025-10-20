using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Common;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Common;

public sealed class ValueObjectTests
{
    private sealed class TestValueObject : ValueObject
    {
        public string Value1 { get; }
        public int Value2 { get; }

        public TestValueObject(string value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value1;
            yield return Value2;
        }
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenValuesAreEqual()
    {
        TestValueObject vo1 = new("test", 123);
        TestValueObject vo2 = new("test", 123);
        vo1.Should().Be(vo2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenValuesAreDifferent()
    {
        TestValueObject vo1 = new("test", 123);
        TestValueObject vo2 = new("test", 456);
        vo1.Should().NotBe(vo2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingWithNull()
    {
        TestValueObject vo1 = new("test", 123);
        vo1.Should().NotBe(null);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEqualObjects()
    {
        TestValueObject vo1 = new("test", 123);
        TestValueObject vo2 = new("test", 123);
        vo1.GetHashCode().Should().Be(vo2.GetHashCode());
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
    {
        TestValueObject vo1 = new("test", 123);
        TestValueObject vo2 = new("test", 123);
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_ShouldReturnTrue_WhenValuesAreDifferent()
    {
        TestValueObject vo1 = new("test", 123);
        TestValueObject vo2 = new("test", 456);
        (vo1 != vo2).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenBothAreNull()
    {
        TestValueObject? vo1 = null;
        TestValueObject? vo2 = null;
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnFalse_WhenOneIsNull()
    {
        TestValueObject? vo1 = new("test", 123);
        TestValueObject? vo2 = null;
        (vo1 == vo2).Should().BeFalse();
    }
}
