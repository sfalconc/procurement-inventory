using Procurement.Domain.Common;

namespace Procurement.Domain.Events.Inventory;

public sealed record LowStockEvent(
    Guid InventoryId,
    Guid ProductId,
    int CurrentQuantity,
    int MinimumStock) : IDomainEvent;
