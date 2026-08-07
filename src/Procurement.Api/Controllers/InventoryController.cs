using MediatR;
using Microsoft.AspNetCore.Mvc;
using Procurement.Application.Inventory.Commands.CreateInventory;
using Procurement.Application.Inventory.Commands.DeleteInventory;
using Procurement.Application.Inventory.Commands.UpdateInventory;
using Procurement.Application.Inventory.DTOs;
using Procurement.Application.Inventory.Queries.GetInventories;
using Procurement.Application.Inventory.Queries.GetInventoryById;
using Procurement.Application.Inventory.Commands.CreateInventoryMovement;
using Procurement.Application.Inventory.Queries.GetLowStockInventories;
using Microsoft.AspNetCore.Authorization;
using Procurement.Application.Common.Constants;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PolicyConstants.RequireWarehouse)]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(
        IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var inventories = await _mediator.Send(
            new GetInventoriesQuery(),
            cancellationToken);

        return Ok(inventories);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InventoryResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var inventory = await _mediator.Send(
            new GetInventoryByIdQuery(id),
            cancellationToken);

        if (inventory is null)
        {
            return NotFound();
        }

        return Ok(inventory);
    }


    [HttpPost]
    public async Task<ActionResult<InventoryResponse>> Create(
        CreateInventoryRequest request,
        CancellationToken cancellationToken)
    {
        var inventory = await _mediator.Send(
            new CreateInventoryCommand(
                request.ProductId,
                request.MinimumStock,
                request.AverageUnitCost),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = inventory.Id },
            inventory);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateInventoryRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateInventoryCommand(
                id,
                request.MinimumStock,
                request.AverageUnitCost),
            cancellationToken);

        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new DeleteInventoryCommand(id),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/movements")]
    public async Task<ActionResult<InventoryMovementResponse>> CreateMovement(
        Guid id,
        CreateInventoryMovementRequest request,
        CancellationToken cancellationToken)
    {
        var movement = await _mediator.Send(
            new CreateInventoryMovementCommand(
                id,
                request.Type,
                request.Quantity,
                request.Description),
            cancellationToken);

        return Ok(movement);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<InventoryResponse>>> GetLowStock(
        CancellationToken cancellationToken)
    {
        var inventories = await _mediator.Send(
            new GetLowStockInventoriesQuery(),
            cancellationToken);

        return Ok(inventories);
    }
}