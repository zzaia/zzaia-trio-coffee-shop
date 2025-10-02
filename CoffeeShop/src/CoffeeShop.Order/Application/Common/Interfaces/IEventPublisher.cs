namespace Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

/// <summary>
/// Defines event publishing operations.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to a specified topic.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="topicName">The topic name to publish to.</param>
    /// <param name="eventData">The event data to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<T>(string topicName, T eventData, CancellationToken cancellationToken = default)
        where T : class;
}
