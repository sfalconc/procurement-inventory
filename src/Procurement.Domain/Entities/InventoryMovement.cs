using Procurement.Domain.Common;
using Procurement.Domain.Enums;

namespace Procurement.Domain.Entities;

public class InventoryMovement : BaseEntity
{
    public Guid InventoryId { get; private set; }

    public Inventory Inventory { get; private set; } = null!;

    public InventoryMovementType Type { get; private set; }

    public int Quantity { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public InventoryMovement(
        Guid inventoryId,
        InventoryMovementType type,
        int quantity,
        string description)
    {
        InventoryId = inventoryId;
        Type = type;
        Quantity = quantity;
        Description = description;
    }
}