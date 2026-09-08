using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Common;

namespace Procurement.UnitTests.Common;

public sealed class TestDomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(
        IReadOnlyCollection<BaseEntity> entities,
        CancellationToken cancellationToken)
    {
        foreach (BaseEntity entity in entities)
        {
            entity.ClearDomainEvents();
        }

        return Task.CompletedTask;
    }
}
