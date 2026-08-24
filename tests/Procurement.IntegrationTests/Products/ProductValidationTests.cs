using System.Net;

using Procurement.Application.Common.Constants;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Products;

[Collection("Integration Tests")]
public class ProductValidationTests : IntegrationTestBase
{
    public ProductValidationTests(
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
    public async Task GetProducts_ShouldReturnOk_WhenUserIsAuthorized()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await Client.GetAsync(
            "/api/products");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await Client.GetAsync(
            $"/api/products/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_ShouldRejectWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var response = await Client.GetAsync(
            "/api/products");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}