using MediatR;
using Microsoft.AspNetCore.Mvc;
using Procurement.Application.Suppliers.Commands.CreateSupplier;
using Procurement.Application.Suppliers.Commands.DeleteSupplier;
using Procurement.Application.Suppliers.Commands.UpdateSupplier;
using Procurement.Application.Suppliers.DTOs;
using Procurement.Application.Suppliers.Queries.GetSupplierById;
using Procurement.Application.Suppliers.Queries.GetSuppliers;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    public async Task<ActionResult<SupplierResponse>> Create(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSupplierCommand(
            request.Code,
            request.Name,
            request.TaxId,
            request.ContactName,
            request.Email,
            request.Phone);


        var supplier = await _mediator.Send(
            command,
            cancellationToken);


        return CreatedAtAction(
            nameof(GetById),
            new { id = supplier.Id },
            supplier);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SupplierResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetSupplierByIdQuery(
            id);


        var supplier = await _mediator.Send(
            query,
            cancellationToken);


        if (supplier is null)
        {
            return NotFound();
        }


        return Ok(supplier);
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var query = new GetSuppliersQuery();


        var suppliers = await _mediator.Send(
            query,
            cancellationToken);


        return Ok(suppliers);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSupplierCommand(
            id,
            request.Code,
            request.Name,
            request.TaxId,
            request.ContactName,
            request.Email,
            request.Phone);


        await _mediator.Send(
            command,
            cancellationToken);


        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSupplierCommand(
            id);


        await _mediator.Send(
            command,
            cancellationToken);


        return NoContent();
    }
}