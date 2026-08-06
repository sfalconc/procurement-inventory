namespace Procurement.Application.Inventory.DTOs;

public class CreateInventoryRequest
{
    public Guid ProductId { get; set; }

    public int MinimumStock { get; set; }

    public decimal AverageUnitCost { get; set; }
}