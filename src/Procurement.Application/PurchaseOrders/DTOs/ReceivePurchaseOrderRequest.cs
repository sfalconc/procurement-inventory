namespace Procurement.Application.PurchaseOrders.DTOs;

public class ReceivePurchaseOrderRequest
{
    public List<ReceivePurchaseOrderLineRequest> Lines { get; set; } = [];
}