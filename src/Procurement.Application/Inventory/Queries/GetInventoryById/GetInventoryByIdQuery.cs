using MediatR;
using Procurement.Application.Inventory.DTOs;

namespace Procurement.Application.Inventory.Queries.GetInventoryById;

public record GetInventoryByIdQuery(
    Guid Id)
    : IRequest<InventoryResponse?>;