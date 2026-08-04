namespace Procurement.Application.Products.DTOs;

public class UpdateProductRequest
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }
}