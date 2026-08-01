public class PurchaseOrder : BaseEntity
{
    public Guid SupplierId { get; private set; }

    public Supplier Supplier { get; private set; } = null!;

    public PurchaseOrderStatus Status { get; private set; }

    public DateTime OrderDate { get; private set; }

    public ICollection<PurchaseOrderLine> Lines { get; private set; } = new List<PurchaseOrderLine>();

    public PurchaseOrder(Guid supplierId)
    {
        SupplierId = supplierId;
        Status = PurchaseOrderStatus.PendingApproval;
        OrderDate = DateTime.UtcNow;
    }
}