using System.Net;
using System.Net.Http.Json;

using Procurement.Application.Common.Constants;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.PurchaseOrders;

[Collection("Integration Tests")]
public class PurchaseOrdersFlowTests : IntegrationTestBase
{
    public PurchaseOrdersFlowTests(
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
    public async Task CreatePurchaseOrder_ShouldReturnCreated_WhenRequestIsValid()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var data = await TestDataSeeder.SeedAsync(
            Factory);

        var request = new
        {
            SupplierId = data.supplier.Id,

            Lines = new[]
            {
                new
                {
                    ProductId = data.product.Id,
                    Quantity = 10,
                    UnitPrice = 25
                }
            }
        };

        var response = await Client.PostAsJsonAsync(
            "/api/purchaseorders",
            request);

        var content = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.Created,
            $"Status: {response.StatusCode}. Body: {content}");
    }

    [Fact]
    public async Task GetPurchaseOrderById_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await Client.GetAsync(
            $"/api/purchaseorders/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CreatePurchaseOrder_ShouldRejectUnauthorizedRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var request = new
        {
            SupplierId = Guid.NewGuid(),

            Lines = Array.Empty<object>()
        };

        var response = await Client.PostAsJsonAsync(
            "/api/purchaseorders",
            request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}