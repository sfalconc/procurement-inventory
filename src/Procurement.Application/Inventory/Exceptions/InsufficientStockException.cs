namespace Procurement.Application.Inventory.Exceptions;

public sealed class InsufficientStockException : Exception
{
    public InsufficientStockException(Guid inventoryId, int requestedQuantity)
        : base($"Insufficient stock for inventory '{inventoryId}'. Requested quantity: {requestedQuantity}.")
    {
        InventoryId = inventoryId;
        RequestedQuantity = requestedQuantity;
    }

    public Guid InventoryId { get; }

    public int RequestedQuantity { get; }
}
