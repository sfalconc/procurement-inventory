using Procurement.Domain.Common;

namespace Procurement.Domain.Events.PurchaseOrders;

public sealed record PurchaseOrderApprovedEvent(Guid PurchaseOrderId) : IDomainEvent;
