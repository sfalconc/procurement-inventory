namespace Procurement.Application.PurchaseOrders.DTOs;

public class PurchaseOrderLineResponse
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public int QuantityReceived { get; set; }

    public decimal UnitPrice { get; set; }

    public int RemainingQuantity { get; set; }
}