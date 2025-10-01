using FluentAssertions;
using Zzaia.CoffeeShop.Order.Domain.Common;

namespace Zzaia.CoffeeShop.Order.Tests.Domain.Common;

public sealed class EntityTests
{
    private sealed class TestEntity : Entity
    {
        public void AddTestEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }
    }

    private sealed record TestDomainEvent : IDomainEvent;

    [Fact]
    public void AddDomainEvent_ShouldAddEventToEntity()
    {
        TestEntity entity = new() { Id = Guid.NewGuid() };
        TestDomainEvent domainEvent = new();
        entity.AddTestEvent(domainEvent);
        entity.DomainEvents.Should().HaveCount(1);
        entity.DomainEvents[0].Should().Be(domainEvent);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        TestEntity entity = new() { Id = Guid.NewGuid() };
        TestDomainEvent event1 = new();
        TestDomainEvent event2 = new();
        entity.AddTestEvent(event1);
        entity.AddTestEvent(event2);
        entity.ClearDomainEvents();
        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvents_ShouldBeReadOnly()
    {
        TestEntity entity = new() { Id = Guid.NewGuid() };
        entity.DomainEvents.Should().BeAssignableTo<IReadOnlyList<IDomainEvent>>();
    }
}
