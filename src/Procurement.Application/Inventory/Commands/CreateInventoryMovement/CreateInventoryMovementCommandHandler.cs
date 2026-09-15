using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.DTOs;
using Procurement.Application.Inventory.Exceptions;
using Procurement.Application.Inventory.Interfaces;
using Procurement.Application.Inventory.Models;
using Procurement.Domain.Entities;
using Procurement.Domain.Enums;

namespace Procurement.Application.Inventory.Commands.CreateInventoryMovement;

public class CreateInventoryMovementCommandHandler
    : IRequestHandler<CreateInventoryMovementCommand, InventoryMovementResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IInventoryStockService _inventoryStockService;

    public CreateInventoryMovementCommandHandler(
        IApplicationDbContext context,
        IInventoryStockService inventoryStockService)
    {
        _context = context;
        _inventoryStockService = inventoryStockService;
    }

    public async Task<InventoryMovementResponse> Handle(
        CreateInventoryMovementCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        await using var transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            if (request.Type == InventoryMovementType.Receipt)
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

                inventory.IncreaseStock(request.Quantity);

                var movement = new InventoryMovement(
                    inventory.Id,
                    request.Type,
                    request.Quantity,
                    request.Description);

                await _context.InventoryMovements.AddAsync(
                    movement,
                    cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

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

            if (request.Type == InventoryMovementType.Adjustment)
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

                inventory.AdjustStock(request.Quantity);

                var movement = new InventoryMovement(
                    inventory.Id,
                    request.Type,
                    request.Quantity,
                    request.Description);

                await _context.InventoryMovements.AddAsync(
                    movement,
                    cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

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

            if (request.Type != InventoryMovementType.Consumption)
            {
                throw new InvalidOperationException(
                    "Movement type not supported.");
            }

            var stockResult = await _inventoryStockService
                .DecreaseStockAsync(
                    request.InventoryId,
                    request.Quantity,
                    cancellationToken);

            switch (stockResult.Status)
            {
                case InventoryStockStatus.InventoryNotFound:
                    throw new KeyNotFoundException(
                        "Inventory not found.");

                case InventoryStockStatus.InsufficientStock:
                    throw new InsufficientStockException(
                        request.InventoryId,
                        request.Quantity);
            }

            var consumptionMovement = new InventoryMovement(
                request.InventoryId,
                request.Type,
                request.Quantity,
                request.Description);

            await _context.InventoryMovements.AddAsync(
                consumptionMovement,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new InventoryMovementResponse
            {
                Id = consumptionMovement.Id,
                InventoryId = consumptionMovement.InventoryId,
                Type = consumptionMovement.Type.ToString(),
                Quantity = consumptionMovement.Quantity,
                Description = consumptionMovement.Description,
                CreatedAt = consumptionMovement.CreatedAt
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
