using Procurement.Domain.Common;

namespace Procurement.Domain.Entities;
public class Supplier : BaseEntity
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string TaxId { get; set; } = null!;

    public string? ContactName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }
    public ICollection<PurchaseOrder> PurchaseOrders { get; private set; } = new List<PurchaseOrder>();
}