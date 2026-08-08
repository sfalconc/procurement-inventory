using Microsoft.EntityFrameworkCore;
using Procurement.Application.Products.Commands.UpdateProduct;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new UpdateProductCommandHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var category = new Category(
            "Electronics",
            null)
        {
            Id = Guid.NewGuid()
        };

        var newCategory = new Category(
            "Computers",
            null)
        {
            Id = Guid.NewGuid()
        };

        var product = new Product(
            "OLD-CODE",
            "Old Name",
            "Old Description",
            category.Id);


        _database.Context.Categories.AddRange(
            category,
            newCategory);

        _database.Context.Products.Add(product);


        await _database.Context.SaveChangesAsync();


        var command = new UpdateProductCommand(
            product.Id,
            "NEW-CODE",
            "New Name",
            "New Description",
            newCategory.Id);


        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);


        // Assert
        var updatedProduct = await _database.Context.Products
            .FirstAsync(
                p => p.Id == product.Id);


        Assert.Equal(
            "NEW-CODE",
            updatedProduct.Code);

        Assert.Equal(
            "New Name",
            updatedProduct.Name);

        Assert.Equal(
            "New Description",
            updatedProduct.Description);

        Assert.Equal(
            newCategory.Id,
            updatedProduct.CategoryId);
    }


    [Fact]
    public async Task Handle_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Guid.NewGuid(),
            "NEW-CODE",
            "New Name",
            "New Description",
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


    [Fact]
    public async Task Handle_ShouldThrowException_WhenCategoryDoesNotExist()
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


        var command = new UpdateProductCommand(
            product.Id,
            "PHONE-UPDATED",
            "Updated Phone",
            "Updated description",
            Guid.NewGuid());


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await _handler.Handle(
                    command,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Category does not exist.",
            exception.Message);


        var productInDb = await _database.Context.Products
            .FirstAsync(
                p => p.Id == product.Id);


        Assert.Equal(
            "PHONE-001",
            productInDb.Code);
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}