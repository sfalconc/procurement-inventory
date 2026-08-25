using MediatR;
using Microsoft.Extensions.Logging;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.PurchaseOrders.DTOs;
using Procurement.Domain.Entities;

namespace Procurement.Application.PurchaseOrders.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandler
    : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreatePurchaseOrderCommandHandler> _logger;

    public CreatePurchaseOrderCommandHandler(
        IApplicationDbContext context,
        ILogger<CreatePurchaseOrderCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PurchaseOrderResponse> Handle(
        CreatePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseOrder = new PurchaseOrder(
            request.SupplierId);

        foreach (var line in request.Lines)
        {
            var purchaseOrderLine = new PurchaseOrderLine(
                line.ProductId,
                line.Quantity,
                line.UnitPrice);

            purchaseOrder.Lines.Add(
                purchaseOrderLine);
        }

        _context.PurchaseOrders.Add(
            purchaseOrder);

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "PurchaseOrderCreated {PurchaseOrderId} {SupplierId} {LineCount}",
            purchaseOrder.Id,
            purchaseOrder.SupplierId,
            purchaseOrder.Lines.Count);

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