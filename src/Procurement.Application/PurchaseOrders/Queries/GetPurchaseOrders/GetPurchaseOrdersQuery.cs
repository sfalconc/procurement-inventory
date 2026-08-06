using MediatR;
using Procurement.Application.PurchaseOrders.DTOs;

namespace Procurement.Application.PurchaseOrders.Queries.GetPurchaseOrders;

public record GetPurchaseOrdersQuery()
    : IRequest<List<PurchaseOrderResponse>>;