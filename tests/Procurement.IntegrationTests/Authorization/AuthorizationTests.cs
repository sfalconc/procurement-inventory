using System.Net;

using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Authorization;

public class AuthorizationTests 
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;


    public AuthorizationTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task InventoryEndpoint_ShouldAllowWarehouseRole()
    {
        TestAuthenticationHandler.Role = "Warehouse";

        var client = _factory.CreateClient();


        var response = await client.GetAsync(
            "/api/inventory");


        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task InventoryEndpoint_ShouldRejectPurchaserRole()
    {
        TestAuthenticationHandler.Role = "Purchaser";

        var client = _factory.CreateClient();


        var response = await client.GetAsync(
            "/api/inventory");


        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }


    [Fact]
    public async Task InventoryEndpoint_ShouldRequireAuthentication()
    {
        TestAuthenticationHandler.Role = null;

        var client = _factory.CreateClient();


        var response = await client.GetAsync(
            "/api/inventory");


        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}