using FluentAssertions;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Events;
using Zzaia.CoffeeShop.Order.Domain.Enums;
using Zzaia.CoffeeShop.Order.Domain.Events;

namespace Zzaia.CoffeeShop.Order.Tests.Application.Events;

/// <summary>
/// Unit tests for OrderStatusChangedEventHandler.
/// </summary>
public sealed class OrderStatusChangedEventHandlerTests
{
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly OrderStatusChangedEventHandler _handler;

    public OrderStatusChangedEventHandlerTests()
    {
        _mockEventPublisher = new Mock<IEventPublisher>();
        _handler = new OrderStatusChangedEventHandler(_mockEventPublisher.Object);
    }

    [Fact]
    public async Task Handle_ShouldPublishEventWithCorrectTopic()
    {
        // Arrange
        OrderStatusChangedEvent domainEvent = new(
            Guid.NewGuid(),
            OrderStatus.Waiting,
            OrderStatus.Preparation,
            DateTimeOffset.UtcNow);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockEventPublisher.Verify(
            x => x.PublishAsync(
                "order.status.changed",
                It.IsAny<object>(),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPublishEventWithCorrectData()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        OrderStatus oldStatus = OrderStatus.Waiting;
        OrderStatus newStatus = OrderStatus.Preparation;
        DateTimeOffset changedAt = DateTimeOffset.UtcNow;

        OrderStatusChangedEvent domainEvent = new(orderId, oldStatus, newStatus, changedAt);

        object? capturedPayload = null;
        _mockEventPublisher
            .Setup(x => x.PublishAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, object, CancellationToken>((_, payload, _) => capturedPayload = payload);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        capturedPayload.Should().NotBeNull();
        string json = System.Text.Json.JsonSerializer.Serialize(capturedPayload);
        json.Should().Contain(orderId.ToString());
        json.Should().Contain("Waiting");
        json.Should().Contain("Preparation");
    }
}
