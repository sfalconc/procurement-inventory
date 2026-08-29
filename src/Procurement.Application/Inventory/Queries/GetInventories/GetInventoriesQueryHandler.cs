using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.DTOs;

namespace Procurement.Application.Inventory.Queries.GetInventories;

public class GetInventoriesQueryHandler
    : IRequestHandler<GetInventoriesQuery, IEnumerable<InventoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoriesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryResponse>> Handle(
        GetInventoriesQuery request,
        CancellationToken cancellationToken)
    {
        var inventories = await _context.Inventories
            .Include(i => i.Product)
            .AsNoTracking()
            .ToListAsync(
                cancellationToken);

        return inventories.Select(inventory => new InventoryResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            ProductCode = inventory.Product.Code,
            ProductName = inventory.Product.Name,
            CurrentQuantity = inventory.CurrentQuantity,
            MinimumStock = inventory.MinimumStock,
            AverageUnitCost = inventory.AverageUnitCost,
            RowVersion = inventory.RowVersion,
            IsLowStock = inventory.CurrentQuantity <= inventory.MinimumStock
        });
    }
}