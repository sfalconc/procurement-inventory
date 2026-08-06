using MediatR;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Commands.ReceivePurchaseOrder;

public record ReceivePurchaseOrderCommand(
    Guid PurchaseOrderId,
    List<ReceivePurchaseOrderLineRequest> Lines)
    : IRequest<PurchaseOrderResponse>;