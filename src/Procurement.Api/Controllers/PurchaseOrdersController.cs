using MediatR;
using Microsoft.AspNetCore.Mvc;
using Procurement.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;
using Procurement.Application.PurchaseOrders.Commands.CancelPurchaseOrder;
using Procurement.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using Procurement.Application.PurchaseOrders.Commands.RejectPurchaseOrder;
using Procurement.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;
using Procurement.Application.PurchaseOrders.Commands.ReceivePurchaseOrder;
using Procurement.Application.PurchaseOrders.Queries.GetPurchaseOrderById;
using Procurement.Application.PurchaseOrders.Queries.GetPurchaseOrders;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IMediator _mediator;


    public PurchaseOrdersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPurchaseOrdersQuery(),
            cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPurchaseOrderByIdQuery(id),
            cancellationToken);


        if (result == null)
        {
            return NotFound();
        }


        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePurchaseOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdatePurchaseOrderCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }


        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }


    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ApprovePurchaseOrderCommand(id),
            cancellationToken);

        return Ok(result);
    }


    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RejectPurchaseOrderCommand(id),
            cancellationToken);

        return Ok(result);
    }


    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CancelPurchaseOrderCommand(id),
            cancellationToken);

        return Ok(result);
    }


    [HttpPost("{id:guid}/receive")]
    public async Task<IActionResult> Receive(
        Guid id,
        ReceivePurchaseOrderCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.PurchaseOrderId)
        {
            return BadRequest();
        }


        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }
}