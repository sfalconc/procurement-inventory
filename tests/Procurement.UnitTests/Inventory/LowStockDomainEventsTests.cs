using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Common.Models;
using Procurement.Application.Inventory.DomainEvents;
using Procurement.Infrastructure.Services;
using Procurement.Domain.Entities;
using Procurement.Domain.Events.Inventory;
using Procurement.UnitTests.Common;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.UnitTests.Inventory;

public class LowStockDomainEventsTests
{
    [Fact]
    public void StockAboveThreshold_ShouldNotRaiseLowStockEvent()
    {
        var inventory = CreateInventory(10);
        inventory.IncreaseStock(11);

        Assert.Empty(inventory.DomainEvents);
    }

    [Fact]
    public void StockCrossingThreshold_ShouldRaiseOneEventWithStockDetails()
    {
        var inventory = CreateInventory(10);
        inventory.IncreaseStock(11);

        inventory.DecreaseStock(1);

        var domainEvent = Assert.IsType<LowStockEvent>(
            Assert.Single(inventory.DomainEvents));
        Assert.Equal(inventory.Id, domainEvent.InventoryId);
        Assert.Equal(inventory.ProductId, domainEvent.ProductId);
        Assert.Equal(10, domainEvent.CurrentQuantity);
        Assert.Equal(10, domainEvent.MinimumStock);
    }

    [Fact]
    public void AlreadyLowStock_ShouldNotRaiseAnotherEvent()
    {
        var inventory = CreateInventory(10);
        inventory.IncreaseStock(11);
        inventory.DecreaseStock(1);
        inventory.ClearDomainEvents();

        inventory.DecreaseStock(1);

        Assert.Empty(inventory.DomainEvents);
    }

    [Fact]
    public void RecoveringAndCrossingAgain_ShouldRaiseANewEvent()
    {
        var inventory = CreateInventory(10);
        inventory.IncreaseStock(11);
        inventory.DecreaseStock(1);
        inventory.ClearDomainEvents();

        inventory.IncreaseStock(2);
        inventory.DecreaseStock(3);

        Assert.IsType<LowStockEvent>(
            Assert.Single(inventory.DomainEvents));
    }

    [Fact]
    public void InvalidStockChange_ShouldNotRaiseLowStockEvent()
    {
        var inventory = CreateInventory(10);

        Assert.Throws<ArgumentException>(
            () => inventory.DecreaseStock(0));

        Assert.Empty(inventory.DomainEvents);
    }

    [Fact]
    public async Task StockServiceFollowedBySaveChanges_ShouldDispatchLowStockEventOnce()
    {
        using var database = new SqliteTestDatabase();
        var category = new Category("Test", null);
        database.Context.Categories.Add(category);
        await database.Context.SaveChangesAsync();

        var product = new Product(
            "TEST-002",
            "Stock test product",
            null,
            category.Id);
        database.Context.Products.Add(product);
        await database.Context.SaveChangesAsync();

        var inventory = new InventoryEntity(
            product.Id,
            10,
            100)
        {
            Id = Guid.NewGuid()
        };
        inventory.IncreaseStock(11);
        database.Context.Inventories.Add(inventory);
        await database.Context.SaveChangesAsync();

        var stockService = new InventoryStockService(database.Context);
        await stockService.DecreaseStockAsync(
            inventory.Id,
            1,
            CancellationToken.None);
        await database.Context.SaveChangesAsync();

        var firstDispatches = database.DomainEventDispatcher.Events
            .OfType<LowStockEvent>()
            .ToList();
        Assert.Single(firstDispatches);
        Assert.Equal(10, firstDispatches[0].CurrentQuantity);
        Assert.Equal(10, inventory.CurrentQuantity);

        await stockService.DecreaseStockAsync(
            inventory.Id,
            1,
            CancellationToken.None);
        Assert.Equal(9, inventory.CurrentQuantity);
        await database.Context.SaveChangesAsync();

        Assert.Single(
            database.DomainEventDispatcher.Events
                .OfType<LowStockEvent>());
    }

    [Fact]
    public async Task Handler_ShouldSendNotificationForLowStockEvent()
    {
        using var database = new SqliteTestDatabase();
        var category = new Category("Test", null);
        database.Context.Categories.Add(category);
        await database.Context.SaveChangesAsync();

        var product = new Product(
            "TEST-001",
            "Test product",
            null,
            category.Id);
        database.Context.Products.Add(product);
        await database.Context.SaveChangesAsync();

        var notificationService = new RecordingNotificationService();
        var handler = new LowStockEventHandler(
            database.Context,
            notificationService);

        var domainEvent = new LowStockEvent(
            Guid.NewGuid(),
            product.Id,
            5,
            10);

        await handler.Handle(
            new DomainEventNotification(domainEvent),
            CancellationToken.None);

        var notification = Assert.Single(notificationService.Notifications);
        Assert.Equal("LowStock", notification.Category);
        Assert.Equal(product.Id, notification.TargetId);
        Assert.Contains("5 units remaining", notification.Content);
        Assert.Contains("10", notification.Content);
    }

    [Fact]
    public async Task Handler_ShouldIgnoreUnrelatedEvents()
    {
        using var database = new SqliteTestDatabase();
        var notificationService = new RecordingNotificationService();
        var handler = new LowStockEventHandler(
            database.Context,
            notificationService);

        await handler.Handle(
            new DomainEventNotification(
                new TestDomainEvent()),
            CancellationToken.None);

        Assert.Empty(notificationService.Notifications);
    }

    private static InventoryEntity CreateInventory(int minimumStock)
    {
        return new InventoryEntity(
            Guid.NewGuid(),
            minimumStock,
            100)
        {
            Id = Guid.NewGuid()
        };
    }

    private sealed class TestDomainEvent : Procurement.Domain.Common.IDomainEvent;

    private sealed class RecordingNotificationService : INotificationService
    {
        public List<NotificationMessage> Notifications { get; } = [];

        public Task SendAsync(
            NotificationMessage notification,
            CancellationToken cancellationToken)
        {
            Notifications.Add(notification);
            return Task.CompletedTask;
        }
    }
}
