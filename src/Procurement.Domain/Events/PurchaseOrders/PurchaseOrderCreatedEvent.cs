using Procurement.Domain.Common;

namespace Procurement.Domain.Events.PurchaseOrders;

public sealed record PurchaseOrderCreatedEvent(Guid PurchaseOrderId) : IDomainEvent;
