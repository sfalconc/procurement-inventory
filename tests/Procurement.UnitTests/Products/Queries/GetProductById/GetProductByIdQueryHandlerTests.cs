using Microsoft.EntityFrameworkCore;
using Procurement.Application.Products.Queries.GetProductById;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.Products.Queries.GetProductById;

public class GetProductByIdQueryHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new GetProductByIdQueryHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var category = new Category(
            "Electronics",
            null)
        {
            Id = Guid.NewGuid()
        };

        var product = new Product(
            "PHONE-001",
            "iPhone",
            "Mobile phone",
            category.Id);


        _database.Context.Categories.Add(category);
        _database.Context.Products.Add(product);

        await _database.Context.SaveChangesAsync();


        var query = new GetProductByIdQuery(
            product.Id);


        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);


        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            product.Id,
            result.Id);

        Assert.Equal(
            "PHONE-001",
            result.Code);

        Assert.Equal(
            "iPhone",
            result.Name);

        Assert.Equal(
            "Electronics",
            result.CategoryName);
    }


    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var query = new GetProductByIdQuery(
            Guid.NewGuid());


        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);


        // Assert
        Assert.Null(result);
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}