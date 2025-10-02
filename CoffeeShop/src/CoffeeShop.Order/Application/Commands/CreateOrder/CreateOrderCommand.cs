namespace Zzaia.CoffeeShop.Order.Application.Commands.CreateOrder;

using MediatR;
using Zzaia.CoffeeShop.Order.Application.Common.Models;

/// <summary>
/// Command to create a new order.
/// </summary>
/// <param name="UserId">The user identifier.</param>
/// <param name="Items">The list of order items.</param>
public record CreateOrderCommand(
    string UserId,
    List<OrderItemRequest> Items
) : IRequest<Result<Guid>>;

/// <summary>
/// Represents an order item request.
/// </summary>
/// <param name="ProductId">The product identifier.</param>
/// <param name="VariationId">The optional product variation identifier.</param>
/// <param name="Quantity">The quantity of the product.</param>
public record OrderItemRequest(
    Guid ProductId,
    Guid? VariationId,
    int Quantity
);
