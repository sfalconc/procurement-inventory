using Procurement.Domain.Common;

namespace Procurement.Domain.Entities;

public class Receipt : BaseEntity
{
    public Guid PurchaseOrderId { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    public DateTime ReceivedDate { get; private set; }


    public ICollection<ReceiptLine> Lines { get; private set; } = new List<ReceiptLine>();
    public Receipt(Guid purchaseOrderId)
    {
        PurchaseOrderId = purchaseOrderId;
        ReceivedDate = DateTime.UtcNow;
    }
}