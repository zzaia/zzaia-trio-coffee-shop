using FluentAssertions;
using Moq;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Events;
using Zzaia.CoffeeShop.Order.Domain.Events;

namespace Zzaia.CoffeeShop.Order.Tests.Application.Events;

/// <summary>
/// Unit tests for OrderCreatedEventHandler.
/// </summary>
public sealed class OrderCreatedEventHandlerTests
{
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly OrderCreatedEventHandler _handler;

    public OrderCreatedEventHandlerTests()
    {
        _mockEventPublisher = new Mock<IEventPublisher>();
        _handler = new OrderCreatedEventHandler(_mockEventPublisher.Object);
    }

    [Fact]
    public async Task Handle_ShouldPublishEventWithCorrectTopic()
    {
        // Arrange
        OrderCreatedEvent domainEvent = new(
            Guid.NewGuid(),
            "user-123",
            100.50m,
            "BRL",
            DateTimeOffset.UtcNow);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockEventPublisher.Verify(
            x => x.PublishAsync(
                "order.created",
                It.IsAny<object>(),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPublishEventWithCorrectData()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        string userId = "user-123";
        decimal totalAmount = 100.50m;
        string currency = "BRL";
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;

        OrderCreatedEvent domainEvent = new(orderId, userId, totalAmount, currency, createdAt);

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
        json.Should().Contain(userId);
        json.Should().Contain("100.5");
    }
}
