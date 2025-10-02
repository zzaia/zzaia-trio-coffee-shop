using MediatR;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Domain.Events;

namespace Zzaia.CoffeeShop.Order.Application.Events;

/// <summary>
/// Handles OrderCreatedEvent and publishes to Kafka.
/// </summary>
public sealed class OrderCreatedEventHandler(IEventPublisher eventPublisher)
    : INotificationHandler<OrderCreatedEvent>
{
    private const string TopicName = "order.created";

    /// <inheritdoc/>
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        object eventPayload = new
        {
            order_id = notification.OrderId,
            user_id = notification.UserId,
            total_amount = notification.TotalAmount.Amount,
            created_at = notification.CreatedAt
        };

        await eventPublisher.PublishAsync(TopicName, eventPayload, cancellationToken);
    }
}
