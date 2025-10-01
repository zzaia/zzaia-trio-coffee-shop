using Zzaia.CoffeeShop.Order.Domain.Common;

namespace Zzaia.CoffeeShop.Order.Domain.ValueObjects;

/// <summary>
/// Represents money with amount and currency.
/// </summary>
public sealed class Money : ValueObject
{
    /// <summary>
    /// Gets the amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency.
    /// </summary>
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));
        }
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Creates a new Money instance.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="currency">The currency.</param>
    /// <returns>A new Money instance.</returns>
    public static Money Create(decimal amount, string currency = "BRL")
    {
        return new Money(amount, currency);
    }

    /// <summary>
    /// Adds two money instances.
    /// </summary>
    /// <param name="other">The other money instance.</param>
    /// <returns>A new Money instance with the sum.</returns>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot add money with different currencies.");
        }
        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Subtracts two money instances.
    /// </summary>
    /// <param name="other">The other money instance.</param>
    /// <returns>A new Money instance with the difference.</returns>
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot subtract money with different currencies.");
        }
        return new Money(Amount - other.Amount, Currency);
    }

    /// <summary>
    /// Multiplies money by a factor.
    /// </summary>
    /// <param name="factor">The multiplication factor.</param>
    /// <returns>A new Money instance with the product.</returns>
    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    /// <summary>
    /// Addition operator.
    /// </summary>
    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }

    /// <summary>
    /// Subtraction operator.
    /// </summary>
    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }

    /// <summary>
    /// Multiplication operator.
    /// </summary>
    public static Money operator *(Money money, decimal factor)
    {
        return money.Multiply(factor);
    }

    /// <summary>
    /// Multiplication operator.
    /// </summary>
    public static Money operator *(decimal factor, Money money)
    {
        return money.Multiply(factor);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
