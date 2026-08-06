using Procurement.Domain.Enums;

namespace Procurement.Application.PurchaseOrders.DTOs;

public class PurchaseOrderResponse
{
    public Guid Id { get; set; }

    public Guid SupplierId { get; set; }

    public PurchaseOrderStatus Status { get; set; }

    public DateTime OrderDate { get; set; }

    public List<PurchaseOrderLineResponse> Lines { get; set; } = [];
}