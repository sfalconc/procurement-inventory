using Procurement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Procurement.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendLowStockNotificationAsync(
        Guid productId,
        string productName,
        int currentQuantity,
        int minimumStock,
        CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Low stock notification sent for product {ProductId} ({ProductName}). Current quantity: {CurrentQuantity}, Minimum stock: {MinimumStock}",
            productId,
            productName,
            currentQuantity,
            minimumStock);

        return Task.CompletedTask;
    }
}