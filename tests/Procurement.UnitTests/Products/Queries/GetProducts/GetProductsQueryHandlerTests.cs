using Procurement.Application.Products.Queries.GetProducts;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.Products.Queries.GetProducts;

public class GetProductsQueryHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new GetProductsQueryHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldReturnProducts()
    {
        // Arrange
        var category = new Category(
            "Electronics",
            null)
        {
            Id = Guid.NewGuid()
        };

        var product1 = new Product(
            "PHONE-001",
            "iPhone",
            "Mobile phone",
            category.Id);

        var product2 = new Product(
            "LAPTOP-001",
            "Laptop Lenovo",
            "Notebook",
            category.Id);


        _database.Context.Categories.Add(category);

        _database.Context.Products.AddRange(
            product1,
            product2);


        await _database.Context.SaveChangesAsync();


        var query = new GetProductsQuery();


        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);


        // Assert
        var products = result.ToList();

        Assert.Equal(
            2,
            products.Count);


        Assert.Contains(
            products,
            p => p.Code == "PHONE-001");


        Assert.Contains(
            products,
            p => p.Code == "LAPTOP-001");


        Assert.All(
            products,
            p => Assert.Equal(
                "Electronics",
                p.CategoryName));
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}