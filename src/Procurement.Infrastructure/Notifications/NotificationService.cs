using Procurement.Application.Common.Interfaces;
using Procurement.Application.Common.Models;
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

    public Task SendAsync(
        NotificationMessage notification,
        CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Notification delivered. Category: {Category}, Title: {Title}, TargetId: {TargetId}, Content: {Content}",
            notification.Category,
            notification.Title,
            notification.TargetId,
            notification.Content);

        return Task.CompletedTask;
    }
}