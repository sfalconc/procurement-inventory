namespace Procurement.Application.PurchaseOrders.DTOs;

public class ReceivePurchaseOrderLineRequest
{
    public Guid PurchaseOrderLineId { get; set; }

    public int QuantityReceived { get; set; }
}