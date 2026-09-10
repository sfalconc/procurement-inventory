using Procurement.Application.Common.Models;

namespace Procurement.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendAsync(
        NotificationMessage notification,
        CancellationToken cancellationToken);
}