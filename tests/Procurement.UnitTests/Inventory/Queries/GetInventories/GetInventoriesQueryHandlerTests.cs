using Procurement.Application.Inventory.Queries.GetInventories;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.UnitTests.Inventory.Queries.GetInventories;

public class GetInventoriesQueryHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly GetInventoriesQueryHandler _handler;


    public GetInventoriesQueryHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new GetInventoriesQueryHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldReturnAllInventories()
    {
        // Arrange
        var category = new Category
        {
            Name = "Electronics",
            Description = "Test category"
        };


        _database.Context.Categories.Add(category);

        await _database.Context.SaveChangesAsync();


        var product1 = new Product(
            "PROD-001",
            "Laptop",
            "Notebook",
            category.Id);


        var product2 = new Product(
            "PROD-002",
            "Mouse",
            "Wireless mouse",
            category.Id);


        _database.Context.Products.AddRange(
            product1,
            product2);

        await _database.Context.SaveChangesAsync();


        var inventory1 = new InventoryEntity(
            product1.Id,
            10,
            1000);


        var inventory2 = new InventoryEntity(
            product2.Id,
            50,
            50);


        _database.Context.Inventories.AddRange(
            inventory1,
            inventory2);


        await _database.Context.SaveChangesAsync();


        // Act
        var result = await _handler.Handle(
            new GetInventoriesQuery(),
            CancellationToken.None);


        // Assert
        var inventories = result.ToList();


        Assert.Equal(
            2,
            inventories.Count);


        Assert.Contains(
            inventories,
            i => i.ProductId == product1.Id);


        Assert.Contains(
            inventories,
            i => i.ProductId == product2.Id);
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}