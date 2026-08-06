using MediatR;
using Procurement.Application.Inventory.DTOs;

namespace Procurement.Application.Inventory.Commands.CreateInventory;

public record CreateInventoryCommand(
    Guid ProductId,
    int MinimumStock,
    decimal AverageUnitCost)
    : IRequest<InventoryResponse>;