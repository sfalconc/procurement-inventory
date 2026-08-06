using MediatR;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Commands.CreatePurchaseOrder;

public record CreatePurchaseOrderCommand(
    Guid SupplierId,
    List<CreatePurchaseOrderLineRequest> Lines)
    : IRequest<PurchaseOrderResponse>;