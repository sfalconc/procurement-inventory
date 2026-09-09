using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Procurement.Application.Common;
using Procurement.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using Procurement.Application.PurchaseOrders.DomainEvents;
using Procurement.Application.PurchaseOrders.DTOs;
using Procurement.Domain.Entities;
using Procurement.Domain.Events.PurchaseOrders;

namespace Procurement.UnitTests.PurchaseOrders;

public class PurchaseOrderDomainEventsTests
{
    [Fact]
    public void Approve_ShouldRaiseApprovedEventWithPurchaseOrderId()
    {
        var purchaseOrder = NewPurchaseOrder();

        purchaseOrder.Approve();

        var domainEvent = Assert.IsType<PurchaseOrderApprovedEvent>(
            Assert.Single(purchaseOrder.DomainEvents));
        Assert.Equal(purchaseOrder.Id, domainEvent.PurchaseOrderId);
    }

    [Fact]
    public void LifecycleTransitions_ShouldRaiseTheMatchingEvents()
    {
        var rejectedOrder = NewPurchaseOrder();
        rejectedOrder.Reject();
        Assert.IsType<PurchaseOrderRejectedEvent>(
            Assert.Single(rejectedOrder.DomainEvents));

        var cancelledOrder = NewPurchaseOrder();
        cancelledOrder.Cancel();
        Assert.IsType<PurchaseOrderCancelledEvent>(
            Assert.Single(cancelledOrder.DomainEvents));

        var partiallyReceivedOrder = NewPurchaseOrder();
        partiallyReceivedOrder.Approve();
        partiallyReceivedOrder.ClearDomainEvents();
        partiallyReceivedOrder.MarkAsPartiallyReceived();
        Assert.IsType<PurchaseOrderPartiallyReceivedEvent>(
            Assert.Single(partiallyReceivedOrder.DomainEvents));

        var completedOrder = NewPurchaseOrder();
        completedOrder.Approve();
        completedOrder.ClearDomainEvents();
        completedOrder.MarkAsCompleted();
        Assert.IsType<PurchaseOrderCompletedEvent>(
            Assert.Single(completedOrder.DomainEvents));
    }

    [Fact]
    public void InvalidTransition_ShouldNotRaiseAnEvent()
    {
        var purchaseOrder = NewPurchaseOrder();

        Assert.Throws<InvalidOperationException>(
            purchaseOrder.MarkAsCompleted);

        Assert.Empty(purchaseOrder.DomainEvents);
    }

    [Fact]
    public async Task CreatePurchaseOrder_ShouldRaiseCreatedEventWithGeneratedId()
    {
        var expectedId = Guid.NewGuid();
        var purchaseOrders = new Mock<DbSet<PurchaseOrder>>();
        PurchaseOrder? addedPurchaseOrder = null;
        purchaseOrders
            .Setup(set => set.Add(It.IsAny<PurchaseOrder>()))
            .Callback<PurchaseOrder>(order =>
            {
                order.Id = expectedId;
                addedPurchaseOrder = order;
            });

        var context = new Mock<Procurement.Application.Common.Interfaces.IApplicationDbContext>();
        context.SetupGet(value => value.PurchaseOrders)
            .Returns(purchaseOrders.Object);
        context.Setup(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreatePurchaseOrderCommandHandler(
            context.Object,
            NullLogger<CreatePurchaseOrderCommandHandler>.Instance);

        await handler.Handle(
            new CreatePurchaseOrderCommand(
                Guid.NewGuid(),
                new List<CreatePurchaseOrderLineRequest>()),
            CancellationToken.None);

        var domainEvent = Assert.IsType<PurchaseOrderCreatedEvent>(
            Assert.Single(addedPurchaseOrder!.DomainEvents));
        Assert.Equal(expectedId, domainEvent.PurchaseOrderId);
    }

    [Fact]
    public async Task EventHandlers_ShouldProcessTheirMatchingEvents()
    {
        var purchaseOrderId = Guid.NewGuid();
        var notification = new DomainEventNotification(
            new PurchaseOrderApprovedEvent(purchaseOrderId));
        var logger = new RecordingLogger<PurchaseOrderApprovedEventHandler>();
        var handler = new PurchaseOrderApprovedEventHandler(logger);

        await handler.Handle(notification, CancellationToken.None);

        Assert.Contains(
            purchaseOrderId.ToString(),
            Assert.Single(logger.Messages));
    }

    [Fact]
    public async Task EventHandlers_ShouldIgnoreOtherPurchaseOrderEvents()
    {
        var logger = new RecordingLogger<PurchaseOrderApprovedEventHandler>();
        var handler = new PurchaseOrderApprovedEventHandler(logger);

        await handler.Handle(
            new DomainEventNotification(
                new PurchaseOrderCancelledEvent(Guid.NewGuid())),
            CancellationToken.None);

        Assert.Empty(logger.Messages);
    }

    private static PurchaseOrder NewPurchaseOrder()
    {
        var purchaseOrder = new PurchaseOrder(Guid.NewGuid())
        {
            Id = Guid.NewGuid()
        };
        return purchaseOrder;
    }

    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
