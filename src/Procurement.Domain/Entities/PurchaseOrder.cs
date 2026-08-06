using Procurement.Domain.Common;
using Procurement.Domain.Enums;

namespace Procurement.Domain.Entities;

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
        Status = PurchaseOrderStatus.Draft;
        OrderDate = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("Only draft purchase orders can be approved.");

        Status = PurchaseOrderStatus.Approved;
    }

    public void Reject()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("Only draft purchase orders can be rejected.");

        Status = PurchaseOrderStatus.Rejected;
    }

    public void Cancel()
    {
        if (Status == PurchaseOrderStatus.Completed)
            throw new InvalidOperationException("Completed purchase orders cannot be cancelled.");

        Status = PurchaseOrderStatus.Cancelled;
    }

    public void MarkAsPartiallyReceived()
    {
        if (Status != PurchaseOrderStatus.Approved)
            throw new InvalidOperationException("Purchase order must be approved.");

        Status = PurchaseOrderStatus.PartiallyReceived;
    }

    public void MarkAsCompleted()
    {
        if (Status != PurchaseOrderStatus.Approved &&
            Status != PurchaseOrderStatus.PartiallyReceived)
            throw new InvalidOperationException("Purchase order is not ready to complete.");

        Status = PurchaseOrderStatus.Completed;
    }

    public void SetStatus(
        PurchaseOrderStatus status)
    {
        if (!CanChangeStatus(status))
        {
            throw new InvalidOperationException(
                $"Cannot change purchase order status from {Status} to {status}.");
        }

        Status = status;
    }

    private bool CanChangeStatus(
        PurchaseOrderStatus newStatus)
    {
        return Status switch
        {
            PurchaseOrderStatus.Draft =>
                newStatus == PurchaseOrderStatus.PendingApproval ||
                newStatus == PurchaseOrderStatus.Cancelled,

            PurchaseOrderStatus.PendingApproval =>
                newStatus == PurchaseOrderStatus.Approved ||
                newStatus == PurchaseOrderStatus.Rejected ||
                newStatus == PurchaseOrderStatus.Cancelled,

            PurchaseOrderStatus.Approved =>
                newStatus == PurchaseOrderStatus.PartiallyReceived ||
                newStatus == PurchaseOrderStatus.Completed ||
                newStatus == PurchaseOrderStatus.Cancelled,

            PurchaseOrderStatus.PartiallyReceived =>
                newStatus == PurchaseOrderStatus.Completed ||
                newStatus == PurchaseOrderStatus.Cancelled,

            PurchaseOrderStatus.Completed =>
                false,

            PurchaseOrderStatus.Rejected =>
                false,

            PurchaseOrderStatus.Cancelled =>
                false,

            _ => false
        };
    }
    public void UpdateSupplier(Guid supplierId)
    {
        SupplierId = supplierId;
    }


    public void AddLine(
        Guid productId,
        int quantity,
        decimal unitPrice)
    {
        Lines.Add(
            new PurchaseOrderLine(
                productId,
                quantity,
                unitPrice));
    }


    public void ClearLines()
    {
        Lines.Clear();
    }

}