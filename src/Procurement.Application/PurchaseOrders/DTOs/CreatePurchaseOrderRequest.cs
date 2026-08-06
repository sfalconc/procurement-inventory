namespace Procurement.Application.PurchaseOrders.DTOs;

public class CreatePurchaseOrderRequest
{
    public Guid SupplierId { get; set; }

    public List<CreatePurchaseOrderLineRequest> Lines { get; set; } = [];
}