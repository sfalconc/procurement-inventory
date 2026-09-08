using MediatR;
using Procurement.Application.Common;
using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Common;

namespace Procurement.Infrastructure.DomainEvents;

public sealed class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public MediatRDomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAsync(
        IReadOnlyCollection<BaseEntity> entities,
        CancellationToken cancellationToken)
    {
        foreach (BaseEntity entity in entities)
        {
            IReadOnlyCollection<IDomainEvent> domainEvents =
                entity.DomainEvents.ToArray();

            foreach (IDomainEvent domainEvent in domainEvents)
            {
                await _publisher.Publish(
                    new DomainEventNotification(domainEvent),
                    cancellationToken);
            }

            entity.ClearDomainEvents();
        }
    }
}
