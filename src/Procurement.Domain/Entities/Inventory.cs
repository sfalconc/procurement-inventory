using Procurement.Domain.Common;
using Procurement.Domain.Events.Inventory;

namespace Procurement.Domain.Entities;

public class Inventory : BaseEntity
{
    public Guid ProductId { get; private set; }

    public int CurrentQuantity { get; private set; }

    public int MinimumStock { get; private set; }

    public decimal AverageUnitCost { get; private set; }

    public byte[] RowVersion { get; private set; }
    public Product Product { get; private set; } = null!;

    public ICollection<InventoryMovement> Movements { get; private set; } = new List<InventoryMovement>();

    public void UpdateDetails(
        int minimumStock,
        decimal averageUnitCost)
    {
        var previousMinimumStock = MinimumStock;
        MinimumStock = minimumStock;
        AverageUnitCost = averageUnitCost;
        RaiseLowStockEventIfTransitioned(
            CurrentQuantity,
            previousMinimumStock,
            CurrentQuantity,
            MinimumStock);
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

        var previousQuantity = CurrentQuantity;
        CurrentQuantity += quantity;
        RaiseLowStockEventIfTransitioned(
            previousQuantity,
            MinimumStock,
            CurrentQuantity,
            MinimumStock);
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

        var previousQuantity = CurrentQuantity;
        CurrentQuantity -= quantity;
        RaiseLowStockEventIfTransitioned(
            previousQuantity,
            MinimumStock,
            CurrentQuantity,
            MinimumStock);
    }

    public void AdjustStock(int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new ArgumentException(
                "Stock quantity cannot be negative.");
        }

        var previousQuantity = CurrentQuantity;
        CurrentQuantity = newQuantity;
        RaiseLowStockEventIfTransitioned(
            previousQuantity,
            MinimumStock,
            CurrentQuantity,
            MinimumStock);
    }

    public void SetMinimumStock(int minimumStock)
    {
        var previousMinimumStock = MinimumStock;
        MinimumStock = minimumStock;
        RaiseLowStockEventIfTransitioned(
            CurrentQuantity,
            previousMinimumStock,
            CurrentQuantity,
            MinimumStock);
    }

    public void RecordStockChange(
        int previousQuantity,
        int newQuantity)
    {
        CurrentQuantity = newQuantity;
        RaiseLowStockEventIfTransitioned(
            previousQuantity,
            MinimumStock,
            newQuantity,
            MinimumStock);
    }

    private void RaiseLowStockEventIfTransitioned(
        int previousQuantity,
        int previousMinimumStock,
        int newQuantity,
        int newMinimumStock)
    {
        if (previousQuantity > previousMinimumStock &&
            newQuantity <= newMinimumStock)
        {
            AddDomainEvent(
                new LowStockEvent(
                    Id,
                    ProductId,
                    newQuantity,
                    newMinimumStock));
        }
    }
}