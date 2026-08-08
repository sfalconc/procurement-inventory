using System.Net;
using Procurement.Application.Common.Constants;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Authorization;

public class AuthorizationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthorizationTests(
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
    public async Task InventoryEndpoint_ShouldAllowWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var response = await _client.GetAsync(
            "/api/inventory");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task InventoryEndpoint_ShouldRejectPurchaserRole()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await _client.GetAsync(
            "/api/inventory");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task InventoryEndpoint_ShouldRequireAuthentication()
    {
        _client.DefaultRequestHeaders.Remove(
            "X-Test-Role");

        var response = await _client.GetAsync(
            "/api/inventory");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
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
}