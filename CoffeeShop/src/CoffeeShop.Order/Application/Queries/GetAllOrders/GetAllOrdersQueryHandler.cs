namespace Zzaia.CoffeeShop.Order.Application.Queries.GetAllOrders;

using MediatR;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Application.Queries.GetOrderById;

/// <summary>
/// Handles the GetAllOrdersQuery request.
/// </summary>
public class GetAllOrdersQueryHandler(
    IOrderRepository orderRepository,
    ILogger<GetAllOrdersQueryHandler> logger) : IRequestHandler<GetAllOrdersQuery, Result<List<OrderDto>>>
{
    /// <summary>
    /// Handles the query execution.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the list of order data transfer objects.</returns>
    public async Task<Result<List<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            List<Domain.Entities.Order> orders = await orderRepository.GetAllAsync(cancellationToken);
            List<OrderDto> orderDtos = orders.Select(order => new OrderDto(
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
            )).ToList();
            logger.LogInformation("Retrieved {OrderCount} orders for manager {UserId}", orders.Count, request.RequestingUserId);
            return Result<List<OrderDto>>.Success(orderDtos);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error retrieving all orders");
            return Result<List<OrderDto>>.Failure($"Failed to retrieve orders: {exception.Message}");
        }
    }
}
