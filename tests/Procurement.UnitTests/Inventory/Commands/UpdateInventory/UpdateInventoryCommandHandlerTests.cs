using Procurement.Application.Inventory.Commands.UpdateInventory;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.UnitTests.Inventory.Commands.UpdateInventory;

public class UpdateInventoryCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly UpdateInventoryCommandHandler _handler;


    public UpdateInventoryCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new UpdateInventoryCommandHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldUpdateInventory_WhenExists()
    {
        var category = new Category(
            "Electronics",
            "Test category"
            );

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
            20,
            1000);

        _database.Context.Inventories.Add(inventory);

        await _database.Context.SaveChangesAsync();


        var command = new UpdateInventoryCommand(
            inventory.Id,
            50,
            1200);


        var result = await _handler.Handle(
            command,
            CancellationToken.None);


        Assert.Equal(
            50,
            result.MinimumStock);

        Assert.Equal(
            1200,
            result.AverageUnitCost);
    }


    [Fact]
    public async Task Handle_ShouldThrow_WhenInventoryDoesNotExist()
    {
        var command = new UpdateInventoryCommand(
            Guid.NewGuid(),
            10,
            100);


        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () =>
            {
                await _handler.Handle(
                    command,
                    CancellationToken.None);
            });
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}