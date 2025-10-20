using Zzaia.CoffeeShop.Order.Domain.Common;

namespace Zzaia.CoffeeShop.Order.Domain.ValueObjects;

/// <summary>
/// Represents a quantity value.
/// </summary>
public sealed class Quantity : ValueObject
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public int Value { get; }

    private Quantity(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentException("Quantity must be positive.", nameof(value));
        }
        Value = value;
    }

    /// <summary>
    /// Creates a new Quantity instance.
    /// </summary>
    /// <param name="value">The quantity value.</param>
    /// <returns>A new Quantity instance.</returns>
    public static Quantity Create(int value)
    {
        return new Quantity(value);
    }

    /// <summary>
    /// Increases the quantity by a specified amount.
    /// </summary>
    /// <param name="amount">The amount to increase.</param>
    /// <returns>A new Quantity instance with the increased value.</returns>
    public Quantity Increase(int amount)
    {
        return new Quantity(Value + amount);
    }

    /// <summary>
    /// Decreases the quantity by a specified amount.
    /// </summary>
    /// <param name="amount">The amount to decrease.</param>
    /// <returns>A new Quantity instance with the decreased value.</returns>
    public Quantity Decrease(int amount)
    {
        return new Quantity(Value - amount);
    }

    /// <summary>
    /// Addition operator.
    /// </summary>
    public static Quantity operator +(Quantity left, Quantity right)
    {
        return new Quantity(left.Value + right.Value);
    }

    /// <summary>
    /// Subtraction operator.
    /// </summary>
    public static Quantity operator -(Quantity left, Quantity right)
    {
        return new Quantity(left.Value - right.Value);
    }

    /// <summary>
    /// Addition operator with integer.
    /// </summary>
    public static Quantity operator +(Quantity quantity, int value)
    {
        return quantity.Increase(value);
    }

    /// <summary>
    /// Subtraction operator with integer.
    /// </summary>
    public static Quantity operator -(Quantity quantity, int value)
    {
        return quantity.Decrease(value);
    }

    /// <summary>
    /// Implicit conversion to int.
    /// </summary>
    public static implicit operator int(Quantity quantity)
    {
        return quantity.Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
