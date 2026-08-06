using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Queries.GetPurchaseOrderById;

public class GetPurchaseOrderByIdQueryHandler
    : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderResponse?>
{
    private readonly IApplicationDbContext _context;


    public GetPurchaseOrderByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<PurchaseOrderResponse?> Handle(
        GetPurchaseOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Lines)
            .Where(po => po.Id == request.Id)
            .Select(po => new PurchaseOrderResponse
            {
                Id = po.Id,
                SupplierId = po.SupplierId,
                Status = po.Status,
                OrderDate = po.OrderDate,

                Lines = po.Lines
                    .Select(line => new PurchaseOrderLineResponse
                    {
                        ProductId = line.ProductId,
                        Quantity = line.Quantity,
                        QuantityReceived = line.QuantityReceived,
                        UnitPrice = line.UnitPrice,
                        RemainingQuantity =
                            line.Quantity - line.QuantityReceived
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}