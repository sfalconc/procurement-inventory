using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Receipts.DTOs;

namespace Procurement.Application.Receipts.Queries.GetReceiptById;

public class GetReceiptByIdQueryHandler
    : IRequestHandler<GetReceiptByIdQuery, ReceiptResponse?>
{
    private readonly IApplicationDbContext _context;

    public GetReceiptByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReceiptResponse?> Handle(
        GetReceiptByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Receipts
            .Include(r => r.Lines)
            .Where(r => r.Id == request.Id)
            .Select(r => new ReceiptResponse
            {
                Id = r.Id,
                PurchaseOrderId = r.PurchaseOrderId,
                ReceivedDate = r.ReceivedDate,

                Lines = r.Lines
                    .Select(line => new ReceiptLineResponse
                    {
                        PurchaseOrderLineId = line.PurchaseOrderLineId,
                        QuantityReceived = line.QuantityReceived
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}