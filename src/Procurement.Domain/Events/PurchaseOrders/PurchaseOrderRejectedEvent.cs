using Procurement.Domain.Common;

namespace Procurement.Domain.Events.PurchaseOrders;

public sealed record PurchaseOrderRejectedEvent(Guid PurchaseOrderId) : IDomainEvent;
