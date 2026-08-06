using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.DTOs;
using Procurement.Domain.Entities;
using Procurement.Domain.Enums;

namespace Procurement.Application.Inventory.Commands.CreateInventoryMovement;

public class CreateInventoryMovementCommandHandler
    : IRequestHandler<CreateInventoryMovementCommand, InventoryMovementResponse>
{
    private readonly IApplicationDbContext _context;

    public CreateInventoryMovementCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<InventoryMovementResponse> Handle(
        CreateInventoryMovementCommand request,
        CancellationToken cancellationToken)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(
                i => i.Id == request.InventoryId,
                cancellationToken);


        if (inventory is null)
        {
            throw new KeyNotFoundException(
                "Inventory not found.");
        }


        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }


        switch (request.Type)
        {
            case InventoryMovementType.Receipt:

                inventory.IncreaseStock(
                    request.Quantity);

                break;


            case InventoryMovementType.Consumption:

                inventory.DecreaseStock(
                    request.Quantity);

                break;


            case InventoryMovementType.Adjustment:

                inventory.AdjustStock(
                    request.Quantity);

                break;


            default:

                throw new InvalidOperationException(
                    "Movement type not supported.");
        }

        var movement = new InventoryMovement(
            inventory.Id,
            request.Type,
            request.Quantity,
            request.Description);


        await _context.InventoryMovements.AddAsync(
            movement,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        return new InventoryMovementResponse
        {
            Id = movement.Id,
            InventoryId = movement.InventoryId,
            Type = movement.Type.ToString(),
            Quantity = movement.Quantity,
            Description = movement.Description,
            CreatedAt = movement.CreatedAt
        };
    }
}