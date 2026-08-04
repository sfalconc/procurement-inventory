using Procurement.Domain.Common;

namespace Procurement.Domain.Entities;

public class Product : BaseEntity
{
    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public Guid CategoryId { get; private set; }

    public Category Category { get; private set; } = null!;

    public Inventory Inventory { get; private set; } = null!;

    public Product(
        string code,
        string name,
        string? description,
        Guid categoryId)
    {
        Code = code;
        Name = name;
        Description = description;
        CategoryId = categoryId;
    }
    public void UpdateDetails(
    string code,
    string name,
    string? description,
    Guid categoryId)
    {
        Code = code;
        Name = name;
        Description = description;
        CategoryId = categoryId;
    }
}