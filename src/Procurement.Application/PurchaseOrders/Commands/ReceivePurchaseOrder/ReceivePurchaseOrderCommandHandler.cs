using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.PurchaseOrders.DTOs;
using Procurement.Domain.Entities;
using Procurement.Domain.Enums;

namespace Procurement.Application.PurchaseOrders.Commands.ReceivePurchaseOrder;

public class ReceivePurchaseOrderCommandHandler
    : IRequestHandler<ReceivePurchaseOrderCommand, PurchaseOrderResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ReceivePurchaseOrderCommandHandler> _logger;

    public ReceivePurchaseOrderCommandHandler(
        IApplicationDbContext context,
        ILogger<ReceivePurchaseOrderCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
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
            _logger.LogWarning(
                "PurchaseOrderReceiptFailed {PurchaseOrderId} PurchaseOrderNotFound",
                request.PurchaseOrderId);

            throw new KeyNotFoundException(
                "Purchase order not found.");
        }

        if (purchaseOrder.Status != PurchaseOrderStatus.Approved &&
            purchaseOrder.Status != PurchaseOrderStatus.PartiallyReceived)
        {
            _logger.LogWarning(
                "PurchaseOrderReceiptFailed {PurchaseOrderId} {Status} InvalidStatus",
                purchaseOrder.Id,
                purchaseOrder.Status);

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
                _logger.LogWarning(
                    "PurchaseOrderReceiptFailed {PurchaseOrderId} {PurchaseOrderLineId} LineNotFound",
                    purchaseOrder.Id,
                    requestLine.PurchaseOrderLineId);

                throw new KeyNotFoundException(
                    "Purchase order line not found.");
            }

            var remainingQuantity =
                orderLine.Quantity - orderLine.QuantityReceived;

            if (requestLine.QuantityReceived > remainingQuantity)
            {
                _logger.LogWarning(
                    "PurchaseOrderReceiptFailed {PurchaseOrderId} {PurchaseOrderLineId} QuantityExceeded",
                    purchaseOrder.Id,
                    orderLine.Id);

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

        if (fullyReceived)
        {
            purchaseOrder.MarkAsCompleted();
        }
        else
        {
            purchaseOrder.MarkAsPartiallyReceived();
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "PurchaseOrderReceived {PurchaseOrderId} {ReceiptId} {Status} {LineCount}",
            purchaseOrder.Id,
            receipt.Id,
            purchaseOrder.Status,
            receipt.Lines.Count);

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