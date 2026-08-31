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

            return new InventoryStockResult(
                InventoryStockStatus.Success,
                inventoryId,
                newQuantity);
        }

        var inventoryExists = await _context.Inventories
            .AnyAsync(i => i.Id == inventoryId, cancellationToken);

        if (!inventoryExists)
        {
            return new InventoryStockResult(
                InventoryStockStatus.InventoryNotFound,
                inventoryId);
        }

        return new InventoryStockResult(
            InventoryStockStatus.InsufficientStock,
            inventoryId);
    }
}
