using MediatR;
using Procurement.Domain.Common;

namespace Procurement.Application.Common;

public sealed class DomainEventNotification : INotification
{
    public DomainEventNotification(IDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public IDomainEvent DomainEvent { get; }
}
