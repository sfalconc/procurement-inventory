using Procurement.Domain.Common;

namespace Procurement.Domain.Events.PurchaseOrders;

public sealed record PurchaseOrderCancelledEvent(Guid PurchaseOrderId) : IDomainEvent;
