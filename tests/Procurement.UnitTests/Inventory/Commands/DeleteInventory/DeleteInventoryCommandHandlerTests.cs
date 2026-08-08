using Microsoft.EntityFrameworkCore;
using Procurement.Application.Inventory.Commands.DeleteInventory;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.UnitTests.Inventory.Commands.DeleteInventory;

public class DeleteInventoryCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly DeleteInventoryCommandHandler _handler;


    public DeleteInventoryCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new DeleteInventoryCommandHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldDeleteInventory_WhenExists()
    {
        // Arrange
        var category = new Category(
            "Electronics",
            "Test category");

        _database.Context.Categories.Add(category);

        await _database.Context.SaveChangesAsync();


        var product = new Product(
            "PROD-001",
            "Laptop",
            "Test",
            category.Id);


        _database.Context.Products.Add(product);

        await _database.Context.SaveChangesAsync();


        var inventory = new InventoryEntity(
            product.Id,
            10,
            1000);


        _database.Context.Inventories.Add(inventory);

        await _database.Context.SaveChangesAsync();


        // Act
        await _handler.Handle(
            new DeleteInventoryCommand(inventory.Id),
            CancellationToken.None);


        // Assert
        var exists = await _database.Context.Inventories
            .AnyAsync();


        Assert.False(
            exists);
    }

    public void Dispose()
    {
        _database.Dispose();
    }
}