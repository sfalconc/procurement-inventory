using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Common.Models;
using Procurement.Domain.Events.Inventory;

namespace Procurement.Application.Inventory.DomainEvents;

public sealed class LowStockEventHandler
    : INotificationHandler<DomainEventNotification>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public LowStockEventHandler(
        IApplicationDbContext context,
        INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task Handle(
        DomainEventNotification notification,
        CancellationToken cancellationToken)
    {
        if (notification.DomainEvent is not LowStockEvent domainEvent)
        {
            return;
        }

        var productName = await _context.Products
            .Where(product => product.Id == domainEvent.ProductId)
            .Select(product => product.Name)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Product '{domainEvent.ProductId}' not found.");

        await _notificationService.SendAsync(
            new NotificationMessage(
                "LowStock",
                $"Low stock: {productName}",
                $"Product {productName} ({domainEvent.ProductId}) has {domainEvent.CurrentQuantity} units remaining. Minimum stock is {domainEvent.MinimumStock}.",
                domainEvent.ProductId),
            cancellationToken);
    }
}
