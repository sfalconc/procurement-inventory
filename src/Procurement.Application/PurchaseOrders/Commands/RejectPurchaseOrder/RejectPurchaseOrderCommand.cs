using MediatR;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Commands.RejectPurchaseOrder;

public record RejectPurchaseOrderCommand(
    Guid Id)
    : IRequest<PurchaseOrderResponse>;