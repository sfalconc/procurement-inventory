using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.DTOs;

namespace Procurement.Application.Inventory.Queries.GetLowStockInventories;

public class GetLowStockInventoriesQueryHandler
    : IRequestHandler<GetLowStockInventoriesQuery, IEnumerable<InventoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetLowStockInventoriesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<InventoryResponse>> Handle(
        GetLowStockInventoriesQuery request,
        CancellationToken cancellationToken)
    {
        var inventories = await _context.Inventories
            .Include(i => i.Product)
            .Where(i => 
                i.CurrentQuantity <= i.MinimumStock)
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        return inventories.Select(inventory => new InventoryResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            ProductCode = inventory.Product.Code,
            ProductName = inventory.Product.Name,
            CurrentQuantity = inventory.CurrentQuantity,
            MinimumStock = inventory.MinimumStock,
            AverageUnitCost = inventory.AverageUnitCost,
            IsLowStock = true
        });
    }
}