using System.Net;

using Procurement.Application.Common.Constants;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Suppliers;

[Collection("Integration Tests")]
public class SupplierValidationTests : IntegrationTestBase
{
    public SupplierValidationTests(
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
    public async Task GetSuppliers_ShouldReturnOk_WhenUserIsAuthorized()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await Client.GetAsync(
            "/api/suppliers");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetSupplierById_ShouldReturnNotFound_WhenSupplierDoesNotExist()
    {
        AuthenticateAs(RoleConstants.Purchaser);

        var response = await Client.GetAsync(
            $"/api/suppliers/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetSuppliers_ShouldRejectWarehouseRole()
    {
        AuthenticateAs(RoleConstants.Warehouse);

        var response = await Client.GetAsync(
            "/api/suppliers");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}