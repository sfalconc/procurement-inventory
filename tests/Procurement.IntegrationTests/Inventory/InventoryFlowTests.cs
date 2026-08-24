using System.Net;

using Procurement.Application.Common.Constants;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Inventory;

[Collection("Integration Tests")]
public class InventoryFlowTests
    : IntegrationTestBase
{
    public InventoryFlowTests(
        SqlServerTestFixture fixture)
        : base(fixture)
    {
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