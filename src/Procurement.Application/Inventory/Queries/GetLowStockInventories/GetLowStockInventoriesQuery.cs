using MediatR;
using Procurement.Application.Inventory.DTOs;

namespace Procurement.Application.Inventory.Queries.GetLowStockInventories;

public record GetLowStockInventoriesQuery()
    : IRequest<IEnumerable<InventoryResponse>>;