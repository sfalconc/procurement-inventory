using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.PurchaseOrders.DTOs;
using Procurement.Domain.Entities;
using Procurement.Domain.Enums;

namespace Procurement.Application.PurchaseOrders.Commands.ReceivePurchaseOrder;

public class ReceivePurchaseOrderCommandHandler
    : IRequestHandler<ReceivePurchaseOrderCommand, PurchaseOrderResponse>
{
    private readonly IApplicationDbContext _context;


    public ReceivePurchaseOrderCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<PurchaseOrderResponse> Handle(
        ReceivePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Lines)
            .FirstOrDefaultAsync(
                po => po.Id == request.PurchaseOrderId,
                cancellationToken);


        if (purchaseOrder == null)
        {
            throw new KeyNotFoundException(
                "Purchase order not found.");
        }


        if (purchaseOrder.Status != PurchaseOrderStatus.Approved &&
            purchaseOrder.Status != PurchaseOrderStatus.PartiallyReceived)
        {
            throw new InvalidOperationException(
                "Purchase order cannot be received in current state.");
        }


        var receipt = new Receipt(
            purchaseOrder.Id);


        foreach (var requestLine in request.Lines)
        {
            var orderLine = purchaseOrder.Lines
                .FirstOrDefault(
                    x => x.Id == requestLine.PurchaseOrderLineId);


            if (orderLine == null)
            {
                throw new KeyNotFoundException(
                    "Purchase order line not found.");
            }


            var remainingQuantity =
                orderLine.Quantity - orderLine.QuantityReceived;


            if (requestLine.QuantityReceived > remainingQuantity)
            {
                throw new InvalidOperationException(
                    "Cannot receive more than ordered quantity.");
            }


            orderLine.ReceiveQuantity(
                requestLine.QuantityReceived);


            receipt.Lines.Add(
                new ReceiptLine(
                    orderLine.Id,
                    requestLine.QuantityReceived));


            var inventory = await _context.Inventories
                .FirstAsync(
                    x => x.ProductId == orderLine.ProductId,
                    cancellationToken);


            inventory.IncreaseStock(
                requestLine.QuantityReceived);
        }


        _context.Receipts.Add(
            receipt);


        var fullyReceived = purchaseOrder.Lines
            .All(x => x.QuantityReceived == x.Quantity);


        purchaseOrder.SetStatus(
            fullyReceived
                ? PurchaseOrderStatus.Completed
                : PurchaseOrderStatus.PartiallyReceived);


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