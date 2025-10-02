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
            Domain.Entities.Order order = Domain.Entities.Order.Create(request.UserId);
            foreach (OrderItemRequest itemRequest in request.Items)
            {
                Product? product = await productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);
                if (product is null)
                {
                    return Result<Guid>.Failure($"Product with ID {itemRequest.ProductId} not found");
                }
                if (!product.IsAvailable)
                {
                    return Result<Guid>.Failure($"Product '{product.Name}' is not available");
                }
                Money unitPrice = product.BasePrice;
                string? variationName = null;
                if (itemRequest.VariationId.HasValue)
                {
                    ProductVariation? variation = await productRepository.GetVariationByIdAsync(
                        itemRequest.VariationId.Value,
                        cancellationToken);
                    if (variation is null)
                    {
                        return Result<Guid>.Failure($"Product variation with ID {itemRequest.VariationId.Value} not found");
                    }
                    unitPrice = product.BasePrice.Add(variation.PriceAdjustment);
                    variationName = variation.Name;
                }
                ProductSnapshot snapshot = ProductSnapshot.Create(
                    product.ProductId,
                    product.Name,
                    product.Description,
                    unitPrice,
                    variationName);
                Quantity quantity = Quantity.Create(itemRequest.Quantity);
                order.AddItem(snapshot, quantity);
            }
            await orderRepository.AddAsync(order, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Order {OrderId} created successfully for user {UserId}",
                order.OrderId, order.UserId);
            return Result<Guid>.Success(order.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order for user {UserId}", request.UserId);
            return Result<Guid>.Failure($"Failed to create order: {ex.Message}");
        }
    }
}
