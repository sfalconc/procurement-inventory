namespace Procurement.Application.Suppliers.DTOs;

public class UpdateSupplierRequest
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string TaxId { get; set; } = null!;

    public string? ContactName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }
}