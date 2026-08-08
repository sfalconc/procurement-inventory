using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Receipts.DTOs;

namespace Procurement.Application.Receipts.Queries.GetReceipts;

public class GetReceiptsQueryHandler
    : IRequestHandler<GetReceiptsQuery, List<ReceiptResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetReceiptsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReceiptResponse>> Handle(
        GetReceiptsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Receipts
            .Include(r => r.Lines)
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
            .ToListAsync(cancellationToken);
    }
}