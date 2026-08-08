using Microsoft.EntityFrameworkCore;
using Procurement.Application.Products.Commands.DeleteProduct;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new DeleteProductCommandHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldSoftDeleteProduct_WhenProductExists()
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


        var command = new DeleteProductCommand(
            product.Id);


        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);


        // Assert
        var productInDb = await _database.Context.Products
            .IgnoreQueryFilters()
            .FirstAsync(
                p => p.Id == product.Id);


        Assert.NotNull(productInDb);

        Assert.False(
            productInDb.IsActive);
    }


    [Fact]
    public async Task Handle_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new DeleteProductCommand(
            Guid.NewGuid());


        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () =>
            {
                await _handler.Handle(
                    command,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Product not found.",
            exception.Message);
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}