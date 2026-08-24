using System.Net;

using Procurement.Application.Common.Constants;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Receipts;

[Collection("Integration Tests")]
public class ReceiptsTests : IntegrationTestBase
{
    public ReceiptsTests(
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
    public async Task GetReceipts_ShouldAllowWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var response = await Client.GetAsync(
            "/api/receipts");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetReceipts_ShouldRejectPurchaserRole()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await Client.GetAsync(
            "/api/receipts");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}