namespace Procurement.Application.Inventory.DTOs;

public class InventoryResponse
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int CurrentQuantity { get; set; }

    public int MinimumStock { get; set; }

    public decimal AverageUnitCost { get; set; }

    public bool IsLowStock { get; set; }
}