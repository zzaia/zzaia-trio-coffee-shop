namespace Zzaia.CoffeeShop.Order.Application.Commands.CreateOrder;

using MediatR;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

/// <summary>
/// Handler for CreateOrderCommand.
/// </summary>
public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork,
    ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    /// <summary>
    /// Handles the CreateOrderCommand.
    /// </summary>
    /// <param name="request">The create order command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the order ID if successful.</returns>
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Order order = Order.Create(request.UserId);
            Product? product = await productRepository.GetByIdAsync(request.Item.ProductId, cancellationToken);
            if (product is null)
            {
                return Result<Guid>.Failure($"Product with ID {request.Item.ProductId} not found");
            }
            if (!product.IsAvailable)
            {
                return Result<Guid>.Failure($"Product '{product.Name}' is not available");
            }
            decimal unitPriceAmount = product.BasePriceAmount;
            string currency = product.Currency;
            string? variationName = null;
            if (request.Item.VariationId.HasValue)
            {
                ProductVariation? variation = await productRepository.GetVariationByIdAsync(
                    request.Item.VariationId.Value,
                    cancellationToken);
                if (variation is null)
                {
                    return Result<Guid>.Failure($"Product variation with ID {request.Item.VariationId.Value} not found");
                }
                unitPriceAmount = product.BasePriceAmount + variation.PriceAdjustmentAmount;
                variationName = variation.Name;
            }
            ProductSnapshot snapshot = ProductSnapshot.Create(
                product.ProductId,
                product.Name,
                product.Description,
                unitPriceAmount,
                currency,
                variationName);
            Quantity quantity = Quantity.Create(request.Item.Quantity);
            order.AddItem(snapshot, quantity);

            PaymentRequest paymentRequest = new(
                order.OrderId.ToString(),
                order.TotalAmount,
                order.Currency,
                order.UserId);
            PaymentResult paymentResult = await paymentService.ProcessPaymentAsync(
                paymentRequest,
                cancellationToken);
            if (!paymentResult.Success)
            {
                logger.LogWarning(
                    "Payment failed for order {OrderId}: {Error}",
                    order.OrderId,
                    paymentResult.ErrorMessage);
                return Result<Guid>.Failure(
                    paymentResult.ErrorMessage ?? "Payment processing failed");
            }

            order.SetPaymentTransactionId(paymentResult.TransactionId!);

            try
            {
                await orderRepository.AddAsync(order, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogInformation(
                    "Order {OrderId} created successfully for user {UserId} with transaction {TransactionId}",
                    order.OrderId,
                    order.UserId,
                    paymentResult.TransactionId);
                return Result<Guid>.Success(order.OrderId);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Order creation failed after payment, initiating refund for transaction {TransactionId}",
                    paymentResult.TransactionId);
                await paymentService.RefundPaymentAsync(
                    paymentResult.TransactionId!,
                    order.TotalAmount,
                    order.Currency,
                    cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order for user {UserId}", request.UserId);
            return Result<Guid>.Failure($"Failed to create order: {ex.Message}");
        }
    }
}
