using Procurement.Domain.Common;

namespace Procurement.Domain.Entities;

public class Inventory : BaseEntity
{
    public Guid ProductId { get; private set; }

    public int CurrentQuantity { get; private set; }

    public int MinimumStock { get; private set; }

    public decimal AverageUnitCost { get; private set; }

    public Product Product { get; private set; } = null!;

    public ICollection<InventoryMovement> Movements { get; private set; } = new List<InventoryMovement>();

    public void UpdateDetails(
        int minimumStock,
        decimal averageUnitCost)
    {
        MinimumStock = minimumStock;
        AverageUnitCost = averageUnitCost;
    }

    public Inventory(
        Guid productId,
        int minimumStock,
        decimal averageUnitCost)
    {
        ProductId = productId;
        MinimumStock = minimumStock;
        AverageUnitCost = averageUnitCost;
        CurrentQuantity = 0;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        CurrentQuantity += quantity;
    }


    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        if (CurrentQuantity < quantity)
        {
            throw new InvalidOperationException(
                "Insufficient stock.");
        }

        CurrentQuantity -= quantity;
    }

    public void AdjustStock(int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new ArgumentException(
                "Stock quantity cannot be negative.");
        }

        CurrentQuantity = newQuantity;
    }

    public void SetMinimumStock(int minimumStock)
    {
        MinimumStock = minimumStock;
    }

}