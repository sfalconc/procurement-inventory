using MediatR;
using Procurement.Application.Inventory.DTOs;

namespace Procurement.Application.Inventory.Queries.GetInventories;

public record GetInventoriesQuery
    : IRequest<IEnumerable<InventoryResponse>>;