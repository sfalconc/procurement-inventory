using Procurement.Application.Inventory.Queries.GetLowStockInventories;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.UnitTests.Inventory.Queries.GetLowStockInventories;

public class GetLowStockInventoriesQueryHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly GetLowStockInventoriesQueryHandler _handler;


    public GetLowStockInventoriesQueryHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new GetLowStockInventoriesQueryHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldReturnOnlyLowStockInventories()
    {
        var category = new Category(
            "Electronics",
             "Test category"
        );

        _database.Context.Categories.Add(category);

        await _database.Context.SaveChangesAsync();


        var productLowStock = new Product(
            "PROD-001",
            "Laptop",
            "Test",
            category.Id);


        var productNormalStock = new Product(
            "PROD-002",
            "Mouse",
            "Test",
            category.Id);


        _database.Context.Products.AddRange(
            productLowStock,
            productNormalStock);

        await _database.Context.SaveChangesAsync();


        var lowStockInventory = new InventoryEntity(
            productLowStock.Id,
            10,
            1000);

        lowStockInventory.IncreaseStock(5);


        var normalInventory = new InventoryEntity(
            productNormalStock.Id,
            10,
            1000);

        normalInventory.IncreaseStock(100);

        normalInventory.SetMinimumStock(10);


        _database.Context.Inventories.AddRange(
            lowStockInventory,
            normalInventory);

        await _database.Context.SaveChangesAsync();


        var result = await _handler.Handle(
            new GetLowStockInventoriesQuery(),
            CancellationToken.None);


        var inventories = result.ToList();


        Assert.Single(inventories);

        Assert.Equal(
            productLowStock.Id,
            inventories.First().ProductId);
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}