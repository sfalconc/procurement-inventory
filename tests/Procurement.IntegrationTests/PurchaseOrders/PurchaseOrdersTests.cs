using System.Net;

using Procurement.Application.Common.Constants;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.PurchaseOrders;

[Collection("Integration Tests")]
public class PurchaseOrdersTests : IntegrationTestBase
{
    public PurchaseOrdersTests(
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
    public async Task GetPurchaseOrders_ShouldAllowPurchaserRole()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await Client.GetAsync(
            "/api/purchaseorders");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetPurchaseOrders_ShouldRejectWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var response = await Client.GetAsync(
            "/api/purchaseorders");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}