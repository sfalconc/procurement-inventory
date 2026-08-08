using System.Net;
using System.Net.Http.Json;

using Procurement.Application.Common.Constants;

using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Inventory;

public class InventoryFlowTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InventoryFlowTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }


    private void AuthenticateAs(string role)
    {
        _client.DefaultRequestHeaders.Remove(
            "X-Test-Role");

        _client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            role);
    }


    [Fact]
    public async Task GetInventoryById_ShouldReturnNotFound_WhenInventoryDoesNotExist()
    {
        AuthenticateAs(RoleConstants.Warehouse);


        var response = await _client.GetAsync(
            $"/api/inventory/{Guid.NewGuid()}");


        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    [Fact]
    public async Task GetInventory_ShouldAllowWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);


        var response = await _client.GetAsync(
            "/api/inventory");


        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task GetInventory_ShouldRejectPurchaserRole()
    {
        AuthenticateAs(RoleConstants.Purchaser);


        var response = await _client.GetAsync(
            "/api/inventory");


        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}