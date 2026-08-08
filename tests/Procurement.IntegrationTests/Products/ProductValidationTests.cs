using System.Net;
using System.Net.Http.Json;

using Procurement.Application.Common.Constants;

using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Products;

public class ProductValidationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductValidationTests(
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
    public async Task GetProducts_ShouldReturnOk_WhenUserIsAuthorized()
    {
        AuthenticateAs(RoleConstants.Purchaser);


        var response = await _client.GetAsync(
            "/api/products");


        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        AuthenticateAs(RoleConstants.Purchaser);


        var response = await _client.GetAsync(
            $"/api/products/{Guid.NewGuid()}");


        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    [Fact]
    public async Task GetProducts_ShouldRejectWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);


        var response = await _client.GetAsync(
            "/api/products");


        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}