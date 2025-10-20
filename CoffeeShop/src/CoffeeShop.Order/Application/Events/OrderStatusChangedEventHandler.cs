using MediatR;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Domain.Events;

namespace Zzaia.CoffeeShop.Order.Application.Events;

/// <summary>
/// Handles OrderStatusChangedEvent and publishes to Kafka.
/// </summary>
public sealed class OrderStatusChangedEventHandler(IEventPublisher eventPublisher)
    : INotificationHandler<OrderStatusChangedEvent>
{
    private const string TopicName = "order.status.changed";

    /// <inheritdoc/>
    public async Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        object eventPayload = new
        {
            order_id = notification.OrderId,
            old_status = notification.PreviousStatus.ToString(),
            new_status = notification.NewStatus.ToString(),
            changed_at = notification.ChangedAt
        };

        await eventPublisher.PublishAsync(TopicName, eventPayload, cancellationToken);
    }
}
