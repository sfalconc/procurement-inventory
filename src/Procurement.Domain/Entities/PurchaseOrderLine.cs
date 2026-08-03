using Procurement.Domain.Common;

namespace Procurement.Domain.Entities;

public class PurchaseOrderLine : BaseEntity
{
    public Guid PurchaseOrderId { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    public int QuantityReceived { get; private set; }

    public decimal UnitPrice { get; private set; }

    public PurchaseOrderLine(Guid productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        QuantityReceived = 0;
        UnitPrice = unitPrice;
    }
}