using Microsoft.EntityFrameworkCore;
using Procurement.Application.Inventory.Commands.CreateInventoryMovement;
using Procurement.Domain.Entities;
using Procurement.Domain.Enums;
using Procurement.UnitTests.Common;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.UnitTests.Inventory.Commands.CreateInventoryMovement;

public class CreateInventoryMovementCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly CreateInventoryMovementCommandHandler _handler;


    public CreateInventoryMovementCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new CreateInventoryMovementCommandHandler(
            _database.Context);
    }


    private async Task<Product> CreateProduct()
    {
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
            "Test",
            category.Id);


        _database.Context.Products.Add(product);

        await _database.Context.SaveChangesAsync();

        return product;
    }



    [Fact]
    public async Task Handle_ShouldIncreaseStock_WhenReceiptMovement()
    {
        var product = await CreateProduct();


        var inventory = new InventoryEntity(
            product.Id,
            10,
            1000);

        inventory.IncreaseStock(10);


        _database.Context.Inventories.Add(inventory);

        await _database.Context.SaveChangesAsync();


        await _handler.Handle(
            new CreateInventoryMovementCommand(
                inventory.Id,
                InventoryMovementType.Receipt,
                20,
                "Initial stock receipt"),
            CancellationToken.None);


        var updatedInventory = await _database.Context.Inventories
            .FirstAsync();


        Assert.Equal(
            30,
            updatedInventory.CurrentQuantity);
    }



    [Fact]
    public async Task Handle_ShouldDecreaseStock_WhenConsumptionMovement()
    {
        var product = await CreateProduct();


        var inventory = new InventoryEntity(
            product.Id,
            50,
            1000);


        inventory.IncreaseStock(50);

        _database.Context.Inventories.Add(inventory);

        await _database.Context.SaveChangesAsync();


        await _handler.Handle(
            new CreateInventoryMovementCommand(
                inventory.Id,
                InventoryMovementType.Consumption,
                15,
                "Sale"),
            CancellationToken.None);


        var updatedInventory = await _database.Context.Inventories
            .FirstAsync();


        Assert.Equal(
            35,
            updatedInventory.CurrentQuantity);
    }



    [Fact]
    public async Task Handle_ShouldAdjustStock_WhenAdjustmentMovement()
    {
        var product = await CreateProduct();


        var inventory = new InventoryEntity(
            product.Id,
            100,
            1000);


        inventory.IncreaseStock(100);

        _database.Context.Inventories.Add(inventory);

        await _database.Context.SaveChangesAsync();


        await _handler.Handle(
            new CreateInventoryMovementCommand(
                inventory.Id,
                InventoryMovementType.Adjustment,
                80,
                "Physical inventory count"),
            CancellationToken.None);


        var updatedInventory = await _database.Context.Inventories
            .FirstAsync();


        Assert.Equal(
            80,
            updatedInventory.CurrentQuantity);
    }



    [Fact]
    public async Task Handle_ShouldCreateMovementRecord()
    {
        var product = await CreateProduct();


        var inventory = new InventoryEntity(
            product.Id,
            10,
            1000);


        _database.Context.Inventories.Add(inventory);

        await _database.Context.SaveChangesAsync();


        await _handler.Handle(
            new CreateInventoryMovementCommand(
                inventory.Id,
                InventoryMovementType.Receipt,
                5,
                "Purchase receipt"),
            CancellationToken.None);


        var movementCount = await _database.Context.InventoryMovements
            .CountAsync();


        Assert.Equal(
            1,
            movementCount);
    }



    public void Dispose()
    {
        _database.Dispose();
    }
}