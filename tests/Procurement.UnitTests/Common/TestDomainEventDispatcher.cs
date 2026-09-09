using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Common;

namespace Procurement.UnitTests.Common;

public sealed class TestDomainEventDispatcher : IDomainEventDispatcher
{
    public List<IDomainEvent> Events { get; } = [];

    public Task DispatchAsync(
        IReadOnlyCollection<BaseEntity> entities,
        CancellationToken cancellationToken)
    {
        foreach (BaseEntity entity in entities)
        {
            Events.AddRange(entity.DomainEvents);
            entity.ClearDomainEvents();
        }

        return Task.CompletedTask;
    }
}
