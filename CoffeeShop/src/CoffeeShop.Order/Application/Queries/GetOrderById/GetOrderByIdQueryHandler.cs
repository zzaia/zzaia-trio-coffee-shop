namespace Zzaia.CoffeeShop.Order.Application.Queries.GetOrderById;

using MediatR;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;

/// <summary>
/// Handles the GetOrderByIdQuery request.
/// </summary>
public class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    ILogger<GetOrderByIdQueryHandler> logger) : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    /// <summary>
    /// Handles the query execution.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the order data transfer object.</returns>
    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Order? order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order is null)
            {
                return Result<OrderDto>.Failure($"Order with ID {request.OrderId} not found");
            }
            if (order.UserId != request.RequestingUserId)
            {
                logger.LogWarning("User {UserId} attempted to access order {OrderId} belonging to {OwnerId}",
                    request.RequestingUserId, request.OrderId, order.UserId);
                return Result<OrderDto>.Failure("You are not authorized to view this order");
            }
            OrderDto orderDto = new OrderDto(
                order.OrderId,
                order.UserId,
                order.Items.Select(item => new OrderItemDto(
                    item.OrderItemId,
                    item.ProductSnapshot.ProductId,
                    item.ProductSnapshot.Name,
                    item.ProductSnapshot.Description,
                    item.ProductSnapshot.UnitPriceAmount,
                    item.ProductSnapshot.Currency,
                    item.ProductSnapshot.VariationName,
                    item.Quantity.Value,
                    item.SubtotalAmount
                )).ToList(),
                order.TotalAmount,
                order.Currency,
                order.Status.ToString(),
                order.PaymentTransactionId,
                order.CreatedAt,
                order.UpdatedAt
            );
            logger.LogInformation("Retrieved order {OrderId} for user {UserId}", order.OrderId, order.UserId);
            return Result<OrderDto>.Success(orderDto);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error retrieving order {OrderId}", request.OrderId);
            return Result<OrderDto>.Failure($"Failed to retrieve order: {exception.Message}");
        }
    }
}
