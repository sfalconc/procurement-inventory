using Procurement.Domain.Common;

namespace Procurement.Domain.Events.PurchaseOrders;

public sealed record PurchaseOrderPartiallyReceivedEvent(Guid PurchaseOrderId) : IDomainEvent;
