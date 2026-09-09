using Microsoft.EntityFrameworkCore;
using Procurement.Application.Inventory.Interfaces;
using Procurement.Application.Inventory.Models;
using Procurement.Infrastructure.Persistence;

namespace Procurement.Infrastructure.Services;

public class InventoryStockService : IInventoryStockService
{
    private readonly ProcurementDbContext _context;

    public InventoryStockService(ProcurementDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryStockResult> DecreaseStockAsync(
        Guid inventoryId,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(
                i => i.Id == inventoryId,
                cancellationToken);

        if (inventory is null)
        {
            return new InventoryStockResult(
                InventoryStockStatus.InventoryNotFound,
                inventoryId);
        }

        var previousQuantity = inventory.CurrentQuantity;
        var affectedRows = await _context.Inventories
            .Where(i => i.Id == inventoryId && i.CurrentQuantity >= quantity)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        i => i.CurrentQuantity,
                        i => i.CurrentQuantity - quantity),
                cancellationToken);

        if (affectedRows > 0)
        {
            var newQuantity = await _context.Inventories
                .Where(i => i.Id == inventoryId)
                .Select(i => i.CurrentQuantity)
                .SingleAsync(cancellationToken);

            inventory.RecordStockChange(
                previousQuantity,
                newQuantity);
            var quantityEntry = _context.Entry(inventory)
                .Property(i => i.CurrentQuantity);
            quantityEntry.CurrentValue = newQuantity;
            quantityEntry.OriginalValue = newQuantity;
            quantityEntry.IsModified = false;

            return new InventoryStockResult(
                InventoryStockStatus.Success,
                inventoryId,
                newQuantity);
        }

        return new InventoryStockResult(
            InventoryStockStatus.InsufficientStock,
            inventoryId);
    }
}
