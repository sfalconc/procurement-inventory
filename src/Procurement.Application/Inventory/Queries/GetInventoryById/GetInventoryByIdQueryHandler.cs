using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.DTOs;

namespace Procurement.Application.Inventory.Queries.GetInventoryById;

public class GetInventoryByIdQueryHandler
    : IRequestHandler<GetInventoryByIdQuery, InventoryResponse?>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryResponse?> Handle(
        GetInventoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var inventory = await _context.Inventories
            .Include(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                i => i.Id == request.Id,
                cancellationToken);

        if (inventory is null)
        {
            return null;
        }

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