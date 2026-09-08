using System.Net;

using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Constants;
using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Entities;
using Procurement.Infrastructure.Persistence;
using Procurement.IntegrationTests.Common;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.IntegrationTests.Inventory;

[Collection("Integration Tests")]
public class InventoryFlowTests
    : IntegrationTestBase
{
    private readonly SqlServerTestFixture _fixture;

    public InventoryFlowTests(
        SqlServerTestFixture fixture)
        : base(fixture)
    {
        _fixture = fixture;
    }

    private void AuthenticateAs(string role)
    {
        Client.DefaultRequestHeaders.Remove(
            "X-Test-Role");

        Client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            role);
    }

    [Fact]
    public async Task GetInventory_ShouldAllowWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var response = await Client.GetAsync(
            "/api/inventory");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateInventory_ShouldThrowConcurrencyException_WhenAnotherDbContextHasModifiedTheSameRow()
    {
        var options = new DbContextOptionsBuilder<ProcurementDbContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .Options;

        await using var setupContext = new ProcurementDbContext(
            options,
            new TestCurrentUserService(),
            new TestDomainEventDispatcher());

        var category = new Category(
            "Electronics",
            "Test category");

        setupContext.Categories.Add(category);
        await setupContext.SaveChangesAsync();

        var product = new Product(
            "PROD-ROWVERSION-001",
            "RowVersion Product",
            "Concurrency test",
            category.Id);

        setupContext.Products.Add(product);
        await setupContext.SaveChangesAsync();

        var inventory = new InventoryEntity(
            product.Id,
            10,
            50m);

        setupContext.Inventories.Add(inventory);
        await setupContext.SaveChangesAsync();

        await using var firstContext = new ProcurementDbContext(
            options,
            new TestCurrentUserService(),
            new TestDomainEventDispatcher());

        await using var secondContext = new ProcurementDbContext(
            options,
            new TestCurrentUserService(),
            new TestDomainEventDispatcher());

        var firstInventory = await firstContext.Inventories
            .SingleAsync(i => i.Id == inventory.Id);

        var secondInventory = await secondContext.Inventories
            .SingleAsync(i => i.Id == inventory.Id);

        firstInventory.UpdateDetails(
            15,
            60m);

        await firstContext.SaveChangesAsync();

        secondInventory.UpdateDetails(
            20,
            70m);

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            async () => await secondContext.SaveChangesAsync());
    }

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public string? UserId => "IntegrationTestUser";
    }

    [Fact]
    public async Task GetInventoryById_ShouldReturnNotFound_WhenInventoryDoesNotExist()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var response = await Client.GetAsync(
            $"/api/inventory/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetInventory_ShouldRejectPurchaserRole()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await Client.GetAsync(
            "/api/inventory");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task GetInventory_ShouldAllowSuperUserRole()
    {
        AuthenticateAs(RoleConstants.SuperUser);

        var response = await Client.GetAsync(
            "/api/inventory");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
}