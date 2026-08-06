using MediatR;
using Procurement.Application.Inventory.DTOs;
using Procurement.Domain.Enums;

namespace Procurement.Application.Inventory.Commands.CreateInventoryMovement;

public record CreateInventoryMovementCommand(
    Guid InventoryId,
    InventoryMovementType Type,
    int Quantity,
    string Description)
    : IRequest<InventoryMovementResponse>;