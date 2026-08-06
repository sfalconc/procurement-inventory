namespace Procurement.Application.Inventory.DTOs;

public class UpdateInventoryRequest
{
    public int MinimumStock { get; set; }

    public decimal AverageUnitCost { get; set; }
}