using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.DTOs;

namespace Procurement.Application.Inventory.Commands.UpdateInventory;

public class UpdateInventoryCommandHandler
    : IRequestHandler<UpdateInventoryCommand, InventoryResponse>
{
    private readonly IApplicationDbContext _context;

    public UpdateInventoryCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryResponse> Handle(
        UpdateInventoryCommand request,
        CancellationToken cancellationToken)
    {
        var inventory = await _context.Inventories
            .Include(i => i.Product)
            .FirstOrDefaultAsync(
                i => i.Id == request.Id,
                cancellationToken);

        if (inventory is null)
        {
            throw new KeyNotFoundException(
                "Inventory not found.");
        }

        inventory.UpdateDetails(
            request.MinimumStock,
            request.AverageUnitCost);

        await _context.SaveChangesAsync(
            cancellationToken);

        return new InventoryResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            ProductCode = inventory.Product.Code,
            ProductName = inventory.Product.Name,
            CurrentQuantity = inventory.CurrentQuantity,
            MinimumStock = inventory.MinimumStock,
            AverageUnitCost = inventory.AverageUnitCost,
            IsLowStock = inventory.CurrentQuantity <= inventory.MinimumStock
        };
    }
}