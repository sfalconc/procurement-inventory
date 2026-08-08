using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Receipts.DTOs;
using Procurement.Application.Receipts.Queries.GetReceipts;
using Procurement.Application.Receipts.Queries.GetReceiptById;
using Microsoft.AspNetCore.Authorization;
using Procurement.Application.Common.Constants;

namespace Procurement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = PolicyConstants.RequireWarehouse)]
public class ReceiptsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReceiptsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReceiptResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var receipts = await _mediator.Send(
            new GetReceiptsQuery(),
            cancellationToken);

        return Ok(receipts);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReceiptResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var receipt = await _mediator.Send(
            new GetReceiptByIdQuery(id),
            cancellationToken);

        if (receipt is null)
        {
            return NotFound();
        }

        return Ok(receipt);
    }
}