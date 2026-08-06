using Microsoft.EntityFrameworkCore;
using Procurement.Application.Inventory.Queries.GetInventoryById;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.UnitTests.Inventory.Queries.GetInventoryById;

public class GetInventoryByIdQueryHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly GetInventoryByIdQueryHandler _handler;


    public GetInventoryByIdQueryHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new GetInventoryByIdQueryHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldReturnInventory_WhenExists()
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
            "Notebook",
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
        var result = await _handler.Handle(
            new GetInventoryByIdQuery(inventory.Id),
            CancellationToken.None);


        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            inventory.Id,
            result.Id);

        Assert.Equal(
            product.Id,
            result.ProductId);
    }


    [Fact]
    public async Task Handle_ShouldThrow_WhenInventoryDoesNotExist()
    {
        var query = new GetInventoryByIdQuery(
            Guid.NewGuid());


        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () =>
            {
                await _handler.Handle(
                    query,
                    CancellationToken.None);
            });
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}