using MediatR;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Queries.GetPurchaseOrderById;

public record GetPurchaseOrderByIdQuery(
    Guid Id)
    : IRequest<PurchaseOrderResponse?>;