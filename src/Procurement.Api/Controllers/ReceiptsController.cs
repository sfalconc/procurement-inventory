using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly IApplicationDbContext _context;


    public ReceiptsController(
        IApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var receipts = await _context.Receipts
            .Include(r => r.Lines)
            .Select(r => new
            {
                r.Id,
                r.PurchaseOrderId,
                r.ReceivedDate,

                Lines = r.Lines
                    .Select(line => new
                    {
                        line.PurchaseOrderLineId,
                        line.QuantityReceived
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);


        return Ok(receipts);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var receipt = await _context.Receipts
            .Include(r => r.Lines)
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.PurchaseOrderId,
                r.ReceivedDate,

                Lines = r.Lines
                    .Select(line => new
                    {
                        line.PurchaseOrderLineId,
                        line.QuantityReceived
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);


        if (receipt == null)
        {
            return NotFound();
        }


        return Ok(receipt);
    }
}