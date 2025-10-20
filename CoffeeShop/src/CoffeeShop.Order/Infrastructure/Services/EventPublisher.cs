using Dapr.Client;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Services;

/// <summary>
/// Implements event publishing using Dapr pub/sub.
/// </summary>
public sealed class EventPublisher(
    DaprClient daprClient,
    ILogger<EventPublisher> logger) : IEventPublisher
{
    private const string PubSubName = "order-pubsub";

    /// <inheritdoc/>
    public async Task PublishAsync<T>(
        string topicName,
        T eventData,
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            await daprClient.PublishEventAsync(
                PubSubName,
                topicName,
                eventData,
                cancellationToken);

            logger.LogInformation(
                "Published event to topic {TopicName}: {EventType}",
                topicName,
                typeof(T).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to publish event to topic {TopicName}: {EventType}",
                topicName,
                typeof(T).Name);
            throw;
        }
    }
}
