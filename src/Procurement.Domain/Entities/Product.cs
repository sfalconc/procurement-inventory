public class Product
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;
}