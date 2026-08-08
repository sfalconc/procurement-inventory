using System.Net;
using Procurement.Application.Common.Constants;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Suppliers;

public class SuppliersTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SuppliersTests(
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
    public async Task GetSuppliers_ShouldAllowPurchaserRole()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await _client.GetAsync(
            "/api/suppliers");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task GetSuppliers_ShouldRejectWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var response = await _client.GetAsync(
            "/api/suppliers");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}