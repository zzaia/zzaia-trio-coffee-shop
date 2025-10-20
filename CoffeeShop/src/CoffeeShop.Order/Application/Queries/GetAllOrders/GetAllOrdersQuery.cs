namespace Zzaia.CoffeeShop.Order.Application.Queries.GetAllOrders;

using MediatR;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Application.Queries.GetOrderById;

/// <summary>
/// Represents a query to retrieve all orders.
/// </summary>
/// <param name="RequestingUserId">The requesting user identifier.</param>
/// <param name="IsManager">Indicates whether the requesting user is a manager.</param>
public record GetAllOrdersQuery(
    string RequestingUserId,
    bool IsManager
) : IRequest<Result<List<OrderDto>>>;
