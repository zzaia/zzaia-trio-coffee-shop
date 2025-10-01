namespace Zzaia.CoffeeShop.Order.Domain.Enums;

/// <summary>
/// Represents the status of an order.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order is waiting for payment confirmation.
    /// </summary>
    Waiting = 0,

    /// <summary>
    /// Order is being prepared.
    /// </summary>
    Preparation = 1,

    /// <summary>
    /// Order is ready for pickup or delivery.
    /// </summary>
    Ready = 2,

    /// <summary>
    /// Order has been delivered.
    /// </summary>
    Delivered = 3
}
