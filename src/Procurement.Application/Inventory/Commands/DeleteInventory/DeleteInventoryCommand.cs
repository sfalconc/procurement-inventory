using MediatR;

namespace Procurement.Application.Inventory.Commands.DeleteInventory;

public record DeleteInventoryCommand(
    Guid Id)
    : IRequest<Unit>;