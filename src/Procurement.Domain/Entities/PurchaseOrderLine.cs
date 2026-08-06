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

    public int RemainingQuantity => Quantity - QuantityReceived;

    public PurchaseOrderLine(Guid productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        QuantityReceived = 0;
        UnitPrice = unitPrice;
    }

    public void Receive(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (QuantityReceived + quantity > Quantity)
            throw new InvalidOperationException("Cannot receive more than ordered.");

        QuantityReceived += quantity;
    }

    public void AddReceivedQuantity(int quantity)
    {
        QuantityReceived += quantity;
    }        


    public void ReceiveQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        if (QuantityReceived + quantity > Quantity)
        {
            throw new InvalidOperationException(
                "Cannot receive more than ordered quantity.");
        }

        QuantityReceived += quantity;
    }
    public ICollection<ReceiptLine> ReceiptLines { get; private set; } = new List<ReceiptLine>();
}