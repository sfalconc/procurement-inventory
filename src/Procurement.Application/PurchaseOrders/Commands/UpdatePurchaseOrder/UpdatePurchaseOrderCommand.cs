using MediatR;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;

public record UpdatePurchaseOrderCommand(
    Guid Id,
    Guid SupplierId,
    List<CreatePurchaseOrderLineRequest> Lines)
    : IRequest<PurchaseOrderResponse>;