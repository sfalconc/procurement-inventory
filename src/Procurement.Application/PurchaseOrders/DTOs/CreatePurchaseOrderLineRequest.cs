namespace Procurement.Application.PurchaseOrders.DTOs;

public class CreatePurchaseOrderLineRequest
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}