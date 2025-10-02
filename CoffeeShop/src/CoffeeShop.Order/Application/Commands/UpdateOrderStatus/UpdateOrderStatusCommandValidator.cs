namespace Zzaia.CoffeeShop.Order.Application.Commands.UpdateOrderStatus;

using FluentValidation;

/// <summary>
/// Validator for UpdateOrderStatusCommand.
/// </summary>
public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrderStatusCommandValidator"/> class.
    /// </summary>
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required");
        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid order status");
    }
}
