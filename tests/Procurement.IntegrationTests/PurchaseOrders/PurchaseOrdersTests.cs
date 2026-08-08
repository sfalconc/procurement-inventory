using System.Net;
using Procurement.Application.Common.Constants;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.PurchaseOrders;

public class PurchaseOrdersTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PurchaseOrdersTests(
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
    public async Task GetPurchaseOrders_ShouldAllowPurchaserRole()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await _client.GetAsync(
            "/api/purchaseorders");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task GetPurchaseOrders_ShouldRejectWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var response = await _client.GetAsync(
            "/api/purchaseorders");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}