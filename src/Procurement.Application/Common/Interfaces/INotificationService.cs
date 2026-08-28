namespace Procurement.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendLowStockNotificationAsync(
        Guid productId,
        string productName,
        int currentQuantity,
        int minimumStock,
        CancellationToken cancellationToken);
}