namespace Zzaia.CoffeeShop.Order.Application.Commands.UpdateOrderStatus;

using MediatR;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;

/// <summary>
/// Handler for UpdateOrderStatusCommand.
/// </summary>
public class UpdateOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateOrderStatusCommandHandler> logger) : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    /// <summary>
    /// Handles the UpdateOrderStatusCommand.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Order? order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order is null)
            {
                return Result.Failure($"Order with ID {request.OrderId} not found");
            }
            try
            {
                order.UpdateStatus(request.NewStatus);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
            orderRepository.Update(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Order {OrderId} status updated to {NewStatus}", order.OrderId, request.NewStatus);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order {OrderId} status", request.OrderId);
            return Result.Failure($"Failed to update order status: {ex.Message}");
        }
    }
}
