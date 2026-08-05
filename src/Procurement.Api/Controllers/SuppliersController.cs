using Microsoft.AspNetCore.Mvc;
using Procurement.Application.Suppliers.DTOs;
using Procurement.Application.Suppliers.Interfaces;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(
        ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }


    [HttpPost]
    public async Task<ActionResult<SupplierResponse>> Create(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var supplier = await _supplierService.CreateAsync(
            request,
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
        var supplier = await _supplierService.GetByIdAsync(
            id,
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
        var suppliers = await _supplierService.GetAllAsync(
            cancellationToken);

        return Ok(suppliers);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        await _supplierService.UpdateAsync(
            id,
            request,
            cancellationToken);

        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _supplierService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}