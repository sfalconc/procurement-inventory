using Procurement.Application.Inventory.Models;

namespace Procurement.Application.Inventory.Interfaces;

public interface IInventoryStockService
{
    Task<InventoryStockResult> DecreaseStockAsync(
        Guid inventoryId,
        int quantity,
        CancellationToken cancellationToken);
}
