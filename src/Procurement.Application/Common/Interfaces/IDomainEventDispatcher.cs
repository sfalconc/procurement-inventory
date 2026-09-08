using Procurement.Domain.Common;

namespace Procurement.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IReadOnlyCollection<BaseEntity> entities,
        CancellationToken cancellationToken);
}
