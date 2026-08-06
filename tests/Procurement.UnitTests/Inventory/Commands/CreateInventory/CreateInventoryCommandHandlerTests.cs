using Microsoft.EntityFrameworkCore;
using Procurement.Application.Inventory.Commands.CreateInventory;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.Inventory.Commands.CreateInventory;

public class CreateInventoryCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly CreateInventoryCommandHandler _handler;


    public CreateInventoryCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new CreateInventoryCommandHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldCreateInventory_WhenProductExists()
    {
        // Arrange
        var category = new Category
        {
            Name = "Electronics",
            Description = "Test category"
        };

        _database.Context.Categories.Add(category);

        await _database.Context.SaveChangesAsync();


        var product = new Product(
            "PROD-001",
            "Laptop",
            "Test product",
            category.Id);


        _database.Context.Products.Add(product);

        await _database.Context.SaveChangesAsync();


        var command = new CreateInventoryCommand(
            product.Id,
            10,
            1500);


        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);


        // Assert
        Assert.NotEqual(
            Guid.Empty,
            result.Id);


        Assert.Equal(
            product.Id,
            result.ProductId);


        Assert.Equal(
            10,
            result.MinimumStock);


        var inventory = await _database.Context.Inventories
            .FirstAsync();


        Assert.Equal(
            product.Id,
            inventory.ProductId);
    }



    [Fact]
    public async Task Handle_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new CreateInventoryCommand(
            Guid.NewGuid(),
            10,
            1500);


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


        Assert.Equal(
            0,
            await _database.Context.Inventories.CountAsync());
    }



    public void Dispose()
    {
        _database.Dispose();
    }
}