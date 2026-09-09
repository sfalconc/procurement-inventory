using MediatR;
using Microsoft.Extensions.Logging;
using Procurement.Application.Common;
using Procurement.Domain.Events.PurchaseOrders;

namespace Procurement.Application.PurchaseOrders.DomainEvents;

public sealed class PurchaseOrderCreatedEventHandler
    : INotificationHandler<DomainEventNotification>
{
    private readonly ILogger<PurchaseOrderCreatedEventHandler> _logger;

    public PurchaseOrderCreatedEventHandler(
        ILogger<PurchaseOrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        DomainEventNotification notification,
        CancellationToken cancellationToken)
    {
        if (notification.DomainEvent is PurchaseOrderCreatedEvent domainEvent)
        {
            _logger.LogInformation(
                "PurchaseOrderCreatedDomainEventProcessed {PurchaseOrderId}",
                domainEvent.PurchaseOrderId);
        }

        return Task.CompletedTask;
    }
}

public sealed class PurchaseOrderApprovedEventHandler
    : INotificationHandler<DomainEventNotification>
{
    private readonly ILogger<PurchaseOrderApprovedEventHandler> _logger;

    public PurchaseOrderApprovedEventHandler(
        ILogger<PurchaseOrderApprovedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        DomainEventNotification notification,
        CancellationToken cancellationToken)
    {
        if (notification.DomainEvent is PurchaseOrderApprovedEvent domainEvent)
        {
            _logger.LogInformation(
                "PurchaseOrderApprovedDomainEventProcessed {PurchaseOrderId}",
                domainEvent.PurchaseOrderId);
        }

        return Task.CompletedTask;
    }
}

public sealed class PurchaseOrderRejectedEventHandler
    : INotificationHandler<DomainEventNotification>
{
    private readonly ILogger<PurchaseOrderRejectedEventHandler> _logger;

    public PurchaseOrderRejectedEventHandler(
        ILogger<PurchaseOrderRejectedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        DomainEventNotification notification,
        CancellationToken cancellationToken)
    {
        if (notification.DomainEvent is PurchaseOrderRejectedEvent domainEvent)
        {
            _logger.LogInformation(
                "PurchaseOrderRejectedDomainEventProcessed {PurchaseOrderId}",
                domainEvent.PurchaseOrderId);
        }

        return Task.CompletedTask;
    }
}

public sealed class PurchaseOrderCancelledEventHandler
    : INotificationHandler<DomainEventNotification>
{
    private readonly ILogger<PurchaseOrderCancelledEventHandler> _logger;

    public PurchaseOrderCancelledEventHandler(
        ILogger<PurchaseOrderCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        DomainEventNotification notification,
        CancellationToken cancellationToken)
    {
        if (notification.DomainEvent is PurchaseOrderCancelledEvent domainEvent)
        {
            _logger.LogInformation(
                "PurchaseOrderCancelledDomainEventProcessed {PurchaseOrderId}",
                domainEvent.PurchaseOrderId);
        }

        return Task.CompletedTask;
    }
}

public sealed class PurchaseOrderPartiallyReceivedEventHandler
    : INotificationHandler<DomainEventNotification>
{
    private readonly ILogger<PurchaseOrderPartiallyReceivedEventHandler> _logger;

    public PurchaseOrderPartiallyReceivedEventHandler(
        ILogger<PurchaseOrderPartiallyReceivedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        DomainEventNotification notification,
        CancellationToken cancellationToken)
    {
        if (notification.DomainEvent is PurchaseOrderPartiallyReceivedEvent domainEvent)
        {
            _logger.LogInformation(
                "PurchaseOrderPartiallyReceivedDomainEventProcessed {PurchaseOrderId}",
                domainEvent.PurchaseOrderId);
        }

        return Task.CompletedTask;
    }
}

public sealed class PurchaseOrderCompletedEventHandler
    : INotificationHandler<DomainEventNotification>
{
    private readonly ILogger<PurchaseOrderCompletedEventHandler> _logger;

    public PurchaseOrderCompletedEventHandler(
        ILogger<PurchaseOrderCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        DomainEventNotification notification,
        CancellationToken cancellationToken)
    {
        if (notification.DomainEvent is PurchaseOrderCompletedEvent domainEvent)
        {
            _logger.LogInformation(
                "PurchaseOrderCompletedDomainEventProcessed {PurchaseOrderId}",
                domainEvent.PurchaseOrderId);
        }

        return Task.CompletedTask;
    }
}
