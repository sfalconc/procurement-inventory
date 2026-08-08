namespace Procurement.Application.Receipts.DTOs;

public class ReceiptResponse
{
    public Guid Id { get; set; }

    public Guid PurchaseOrderId { get; set; }

    public DateTime ReceivedDate { get; set; }

    public List<ReceiptLineResponse> Lines { get; set; } = [];
}

public class ReceiptLineResponse
{
    public Guid PurchaseOrderLineId { get; set; }

    public int QuantityReceived { get; set; }
}