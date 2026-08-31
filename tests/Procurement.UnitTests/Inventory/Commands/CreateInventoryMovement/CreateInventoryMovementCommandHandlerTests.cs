using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Inventory.Commands.CreateInventoryMovement;
using Procurement.Application.Inventory.Exceptions;
using Procurement.Application.Inventory.Interfaces;
using Procurement.Application.Inventory.Models;
using Procurement.Domain.Entities;
using Procurement.Domain.Enums;
using Procurement.Infrastructure.Persistence;
using Procurement.Infrastructure.Services;
using Procurement.UnitTests.Common;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.UnitTests.Inventory.Commands.CreateInventoryMovement;

public class CreateInventoryMovementCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly CreateInventoryMovementCommandHandler _handler;
    private readonly IInventoryStockService _stockService;


    public CreateInventoryMovementCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();
        _stockService = new InventoryStockService(_database.Context);

        _handler = new CreateInventoryMovementCommandHandler(
            _database.Context,
            _stockService);
    }


    private async Task<Product> CreateProduct()
    {
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
            .AsNoTracking()
            .FirstAsync();

        Assert.Equal(30, updatedInventory.CurrentQuantity);
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
            .AsNoTracking()
            .FirstAsync();

        Assert.Equal(35, updatedInventory.CurrentQuantity);
    }


    [Fact]
    public async Task Handle_ShouldThrowInsufficientStockException_WhenNotEnoughStock()
    {
        var product = await CreateProduct();

        var inventory = new InventoryEntity(
            product.Id,
            10,
            1000);

        inventory.IncreaseStock(10);

        _database.Context.Inventories.Add(inventory);
        await _database.Context.SaveChangesAsync();

        await Assert.ThrowsAsync<InsufficientStockException>(
            () => _handler.Handle(
                new CreateInventoryMovementCommand(
                    inventory.Id,
                    InventoryMovementType.Consumption,
                    15,
                    "Insufficient stock sale"),
                CancellationToken.None));

        var updatedInventory = await _database.Context.Inventories
            .AsNoTracking()
            .FirstAsync();

        Assert.Equal(10, updatedInventory.CurrentQuantity);
    }


    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenInventoryDoesNotExist()
    {
        var missingInventoryId = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(
                new CreateInventoryMovementCommand(
                    missingInventoryId,
                    InventoryMovementType.Consumption,
                    5,
                    "Missing inventory"),
                CancellationToken.None));
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
            .AsNoTracking()
            .FirstAsync();

        Assert.Equal(80, updatedInventory.CurrentQuantity);
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

        Assert.Equal(1, movementCount);
    }


    [Fact]
    public async Task Handle_ShouldRollback_WhenMovementSaveFails()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ProcurementDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var setupContext = new SqliteInventoryDbContext(
            options,
            new TestCurrentUserService()))
        {
            await setupContext.Database.EnsureCreatedAsync();

            var category = new Category("Electronics", "Rollback category");
            setupContext.Categories.Add(category);
            await setupContext.SaveChangesAsync();

            var product = new Product(
                "PROD-ROLLBACK-2",
                "Rollback Product 2",
                "Rollback product description 2",
                category.Id);

            setupContext.Products.Add(product);
            await setupContext.SaveChangesAsync();

            var inventory = new InventoryEntity(product.Id, 10, 1000);
            inventory.IncreaseStock(10);

            setupContext.Inventories.Add(inventory);
            await setupContext.SaveChangesAsync();
        }

        await using var failingContext = new ThrowingProcurementDbContext(
            options,
            new TestCurrentUserService());

        var inventoryId = await failingContext.Inventories
            .Select(i => i.Id)
            .SingleAsync();

        var failingStockService = new InventoryStockService(failingContext);
        var failingHandler = new CreateInventoryMovementCommandHandler(
            failingContext,
            failingStockService);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => failingHandler.Handle(
                new CreateInventoryMovementCommand(
                    inventoryId,
                    InventoryMovementType.Consumption,
                    5,
                    "Rollback test"),
                CancellationToken.None));

        Assert.Equal("Simulated save failure", exception.Message);

        await using var verificationContext = new SqliteInventoryDbContext(
            options,
            new TestCurrentUserService());

        var verifiedInventory = await verificationContext.Inventories
            .SingleAsync(i => i.Id == inventoryId);

        Assert.Equal(10, verifiedInventory.CurrentQuantity);

        await connection.DisposeAsync();
    }


    [Fact]
    public async Task Handle_ShouldAllowConcurrentSales_WhenStockIsSufficient()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ProcurementDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var setupContext = new SqliteInventoryDbContext(
            options,
            new TestCurrentUserService()))
        {
            await setupContext.Database.EnsureCreatedAsync();

            var category = new Category("Electronics", "Concurrent category");
            setupContext.Categories.Add(category);
            await setupContext.SaveChangesAsync();

            var product = new Product(
                "PROD-CONCURRENT",
                "Concurrent Product",
                "Concurrent product description",
                category.Id);

            setupContext.Products.Add(product);
            await setupContext.SaveChangesAsync();

            var inventory = new InventoryEntity(product.Id, 50, 1000);
            inventory.IncreaseStock(1000);

            setupContext.Inventories.Add(inventory);
            await setupContext.SaveChangesAsync();
        }

        await using var firstContext = new SqliteInventoryDbContext(
            options,
            new TestCurrentUserService());
        await using var secondContext = new SqliteInventoryDbContext(
            options,
            new TestCurrentUserService());

        var inventoryId = await firstContext.Inventories
            .Select(i => i.Id)
            .SingleAsync();

        var firstHandler = new CreateInventoryMovementCommandHandler(
            firstContext,
            new InventoryStockService(firstContext));
        var secondHandler = new CreateInventoryMovementCommandHandler(
            secondContext,
            new InventoryStockService(secondContext));

        var firstTask = firstHandler.Handle(
            new CreateInventoryMovementCommand(
                inventoryId,
                InventoryMovementType.Consumption,
                5,
                "Concurrent sale 1"),
            CancellationToken.None);

        var secondTask = secondHandler.Handle(
            new CreateInventoryMovementCommand(
                inventoryId,
                InventoryMovementType.Consumption,
                2,
                "Concurrent sale 2"),
            CancellationToken.None);

        await Task.WhenAll(firstTask, secondTask);

        await using var finalContext = new SqliteInventoryDbContext(
            options,
            new TestCurrentUserService());

        var finalInventory = await finalContext.Inventories
            .SingleAsync(i => i.Id == inventoryId);

        Assert.Equal(993, finalInventory.CurrentQuantity);

        var movementCount = await finalContext.InventoryMovements
            .CountAsync();

        Assert.Equal(2, movementCount);

        await connection.DisposeAsync();
    }


    public void Dispose()
    {
        _database.Dispose();
    }

    private class SqliteInventoryDbContext : ProcurementDbContext
    {
        public SqliteInventoryDbContext(
            DbContextOptions<ProcurementDbContext> options,
            ICurrentUserService currentUserService)
            : base(options, currentUserService)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (Database.IsSqlite())
            {
                modelBuilder.Entity<InventoryEntity>()
                    .Property(i => i.RowVersion)
                    .HasDefaultValue(new byte[] { 0 })
                    .ValueGeneratedOnAddOrUpdate();
            }
        }
    }

    private sealed class ThrowingProcurementDbContext : SqliteInventoryDbContext
    {
        public ThrowingProcurementDbContext(
            DbContextOptions<ProcurementDbContext> options,
            ICurrentUserService currentUserService)
            : base(options, currentUserService)
        {
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Simulated save failure");
        }
    }

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public string? UserId => "UnitTestUser";
    }
}