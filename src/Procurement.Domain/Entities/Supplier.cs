using Procurement.Domain.Common;

namespace Procurement.Domain.Entities;
public class Supplier : BaseEntity
{
    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string TaxId { get; private set; } = null!;

    public string? ContactName { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public ICollection<PurchaseOrder> PurchaseOrders { get; private set; } = new List<PurchaseOrder>();

    private Supplier()
    {
    }

    public Supplier(
    string code,
    string name,
    string taxId,
    string? contactName,
    string? email,
    string? phone)
    {
        Code = code;
        Name = name;
        TaxId = taxId;
        ContactName = contactName;
        Email = email;
        Phone = phone;
    }

    public void UpdateDetails(
    string code,
    string name,
    string taxId,
    string? contactName,
    string? email,
    string? phone)
    {
        Code = code;
        Name = name;
        TaxId = taxId;
        ContactName = contactName;
        Email = email;
        Phone = phone;
    }
}