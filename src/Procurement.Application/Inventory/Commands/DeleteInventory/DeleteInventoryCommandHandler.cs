using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;

namespace Procurement.Application.Inventory.Commands.DeleteInventory;

public class DeleteInventoryCommandHandler
    : IRequestHandler<DeleteInventoryCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteInventoryCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(
        DeleteInventoryCommand request,
        CancellationToken cancellationToken)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(
                i => i.Id == request.Id,
                cancellationToken);

        if (inventory is null)
        {
            throw new KeyNotFoundException(
                "Inventory not found.");
        }

        _context.Inventories.Remove(
            inventory);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Unit.Value;
    }
}