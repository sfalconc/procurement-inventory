using Microsoft.EntityFrameworkCore;
using Procurement.Application.Products.Commands.CreateProduct;
using Procurement.UnitTests.Common;
using Procurement.Domain.Entities;

namespace Procurement.UnitTests.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new CreateProductCommandHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenCategoryExists()
    {
        // Arrange
        var category = new Category(
            "Electronics",
            "Test category");

        _database.Context.Categories.Add(category);

        await _database.Context.SaveChangesAsync();


        var command = new CreateProductCommand(
            "LAPTOP-001",
            "Laptop Lenovo",
            "Test product",
            category.Id);


        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);


        // Assert
        Assert.NotEqual(
            Guid.Empty,
            result.Id);

        Assert.Equal(
            command.Code,
            result.Code);

        Assert.Equal(
            command.Name,
            result.Name);


        var productInDb = await _database.Context.Products
            .FirstOrDefaultAsync(
                p => p.Id == result.Id);


        Assert.NotNull(productInDb);

        Assert.Equal(
            "Laptop Lenovo",
            productInDb.Name);
    }


    [Fact]
    public async Task Handle_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new CreateProductCommand(
            "LAPTOP-002",
            "Laptop Without Category",
            "Invalid product",
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


        var productsCount = await _database.Context.Products
            .CountAsync();


        Assert.Equal(
            0,
            productsCount);
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}