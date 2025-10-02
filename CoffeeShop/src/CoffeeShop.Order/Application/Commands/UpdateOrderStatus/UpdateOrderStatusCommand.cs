namespace Zzaia.CoffeeShop.Order.Application.Commands.UpdateOrderStatus;

using MediatR;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Domain.Enums;

/// <summary>
/// Command to update the status of an order.
/// </summary>
/// <param name="OrderId">The order identifier.</param>
/// <param name="NewStatus">The new status for the order.</param>
public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus
) : IRequest<Result>;
