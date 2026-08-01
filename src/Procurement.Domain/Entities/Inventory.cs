public class Inventory : BaseEntity
{
    public Guid ProductId { get; set; }

    public int CurrentQuantity { get; set; }

    public int MinimumStock { get; set; }

    public decimal AverageUnitCost { get; set; }

    public Product Product { get; set; } = null!;

    public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
}