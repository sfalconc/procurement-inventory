using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;

public class UpdatePurchaseOrderCommandHandler
    : IRequestHandler<UpdatePurchaseOrderCommand, PurchaseOrderResponse>
{
    private readonly IApplicationDbContext _context;


    public UpdatePurchaseOrderCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<PurchaseOrderResponse> Handle(
        UpdatePurchaseOrderCommand request,
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


        purchaseOrder.UpdateSupplier(
            request.SupplierId);


        purchaseOrder.ClearLines();


        foreach (var line in request.Lines)
        {
            purchaseOrder.AddLine(
                line.ProductId,
                line.Quantity,
                line.UnitPrice);
        }


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