using Procurement.Domain.Common;

namespace Procurement.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category()
    {
    }
    
    public Category(
        string name,
        string? description)
    {
        Name = name;
        Description = description;
    }

    public void UpdateDetails(
        string name,
        string? description)
    {
        Name = name;
        Description = description;
    }
}