namespace Procurement.Application.Inventory.DTOs;

public class InventoryMovementResponse
{
    public Guid Id { get; set; }

    public Guid InventoryId { get; set; }

    public string Type { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}