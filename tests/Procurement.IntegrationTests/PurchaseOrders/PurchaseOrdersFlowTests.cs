using System.Net;
using System.Net.Http.Json;

using Procurement.Application.Common.Constants;

using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.PurchaseOrders;

public class PurchaseOrdersFlowTests
: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;


    public PurchaseOrdersFlowTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
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
    public async Task CreatePurchaseOrder_ShouldReturnCreated_WhenRequestIsValid()
    {
        AuthenticateAs(RoleConstants.Purchaser);


        var data = await TestDataSeeder.SeedAsync(
            _factory);


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


        var response = await _client.PostAsJsonAsync(
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


        var response = await _client.GetAsync(
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


        var response = await _client.PostAsJsonAsync(
            "/api/purchaseorders",
            request);


        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}