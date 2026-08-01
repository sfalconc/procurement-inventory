public class ReceiptLine : BaseEntity
{
    public Guid ReceiptId { get; private set; }

    public Receipt Receipt { get; private set; } = null!;

    public Guid PurchaseOrderLineId { get; private set; }

    public PurchaseOrderLine PurchaseOrderLine { get; private set; } = null!;

    public int QuantityReceived { get; private set; }

    public ReceiptLine(Guid purchaseOrderLineId, int quantityReceived)
    {
        PurchaseOrderLineId = purchaseOrderLineId;
        QuantityReceived = quantityReceived;
    }
}