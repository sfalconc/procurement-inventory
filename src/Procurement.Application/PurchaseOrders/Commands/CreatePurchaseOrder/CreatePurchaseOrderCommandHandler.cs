using MediatR;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.PurchaseOrders.DTOs;
using Procurement.Domain.Entities;

namespace Procurement.Application.PurchaseOrders.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandler
    : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderResponse>
{
    private readonly IApplicationDbContext _context;


    public CreatePurchaseOrderCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
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