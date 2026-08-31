namespace Procurement.Application.Inventory.Models;

public sealed record InventoryStockResult(
    InventoryStockStatus Status,
    Guid InventoryId,
    int? NewQuantity = null);
