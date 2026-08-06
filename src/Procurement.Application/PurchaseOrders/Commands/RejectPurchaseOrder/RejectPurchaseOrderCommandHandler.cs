using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.PurchaseOrders.DTOs;
using Procurement.Domain.Enums;

namespace Procurement.Application.PurchaseOrders.Commands.RejectPurchaseOrder;

public class RejectPurchaseOrderCommandHandler
    : IRequestHandler<RejectPurchaseOrderCommand, PurchaseOrderResponse>
{
    private readonly IApplicationDbContext _context;


    public RejectPurchaseOrderCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<PurchaseOrderResponse> Handle(
        RejectPurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Lines)
            .FirstOrDefaultAsync(
                po => po.Id == request.Id,
                cancellationToken);


        if (purchaseOrder == null)
        {
            throw new KeyNotFoundException(
                "Purchase order not found.");
        }


        if (purchaseOrder.Status != PurchaseOrderStatus.PendingApproval)
        {
            throw new InvalidOperationException(
                "Purchase order cannot be rejected in current state.");
        }


        purchaseOrder.SetStatus(
            PurchaseOrderStatus.Rejected);


        await _context.SaveChangesAsync(
            cancellationToken);


        return new PurchaseOrderResponse
        {
            Id = purchaseOrder.Id,
            SupplierId = purchaseOrder.SupplierId,
            Status = purchaseOrder.Status,
            OrderDate = purchaseOrder.OrderDate,

            Lines = purchaseOrder.Lines
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
        };
    }
}