using Procurement.Domain.Enums;

namespace Procurement.Application.Inventory.DTOs;

public class CreateInventoryMovementRequest
{
    public InventoryMovementType Type { get; set; }

    public int Quantity { get; set; }

    public string Description { get; set; } = string.Empty;
}