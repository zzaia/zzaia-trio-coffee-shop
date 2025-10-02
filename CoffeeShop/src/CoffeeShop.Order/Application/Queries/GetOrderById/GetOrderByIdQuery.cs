namespace Zzaia.CoffeeShop.Order.Application.Queries.GetOrderById;

using MediatR;
using Zzaia.CoffeeShop.Order.Application.Common.Models;

/// <summary>
/// Represents a query to retrieve an order by its identifier.
/// </summary>
/// <param name="OrderId">The order identifier.</param>
/// <param name="RequestingUserId">The requesting user identifier.</param>
public record GetOrderByIdQuery(
    Guid OrderId,
    string RequestingUserId
) : IRequest<Result<OrderDto>>;
