using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.DTOs;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.Application.Inventory.Commands.CreateInventory;

public class CreateInventoryCommandHandler
    : IRequestHandler<CreateInventoryCommand, InventoryResponse>
{
    private readonly IApplicationDbContext _context;

    public CreateInventoryCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryResponse> Handle(
        CreateInventoryCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.ProductId,
                cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        var inventoryExists = await _context.Inventories
            .AnyAsync(
                i => i.ProductId == request.ProductId,
                cancellationToken);

        if (inventoryExists)
        {
            throw new InvalidOperationException(
                "Inventory already exists for this product.");
        }

        var inventory = new InventoryEntity(
            request.ProductId,
            request.MinimumStock,
            request.AverageUnitCost);

        await _context.Inventories.AddAsync(
            inventory,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return new InventoryResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            ProductCode = product.Code,
            ProductName = product.Name,
            CurrentQuantity = inventory.CurrentQuantity,
            MinimumStock = inventory.MinimumStock,
            AverageUnitCost = inventory.AverageUnitCost,
            IsLowStock = inventory.CurrentQuantity <= inventory.MinimumStock
        };
    }
}