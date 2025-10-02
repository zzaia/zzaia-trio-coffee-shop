namespace Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

/// <summary>
/// Defines payment service operations for order processing.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Processes payment for an order asynchronously.
    /// </summary>
    /// <param name="request">The payment request details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The payment result containing transaction details.</returns>
    Task<PaymentResult> ProcessPaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refunds a payment transaction asynchronously.
    /// </summary>
    /// <param name="transactionId">The original transaction identifier.</param>
    /// <param name="amount">The refund amount.</param>
    /// <param name="currency">The currency code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refund result containing transaction details.</returns>
    Task<PaymentResult> RefundPaymentAsync(
        string transactionId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a payment request for order processing.
/// </summary>
/// <param name="OrderId">The order identifier.</param>
/// <param name="Amount">The payment amount.</param>
/// <param name="Currency">The currency code.</param>
/// <param name="UserId">The user identifier.</param>
public record PaymentRequest(
    string OrderId,
    decimal Amount,
    string Currency,
    string UserId);

/// <summary>
/// Represents the result of a payment operation.
/// </summary>
/// <param name="Success">Indicates whether the payment was successful.</param>
/// <param name="TransactionId">The transaction identifier if successful.</param>
/// <param name="ErrorMessage">The error message if unsuccessful.</param>
public record PaymentResult(
    bool Success,
    string? TransactionId,
    string? ErrorMessage);
