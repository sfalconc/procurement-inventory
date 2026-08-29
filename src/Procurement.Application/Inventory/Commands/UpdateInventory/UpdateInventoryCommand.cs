using MediatR;
using Procurement.Application.Inventory.DTOs;

namespace Procurement.Application.Inventory.Commands.UpdateInventory;

public record UpdateInventoryCommand(
    Guid Id,
    int MinimumStock,
    decimal AverageUnitCost,
    byte[] RowVersion)
    : IRequest<InventoryResponse>;