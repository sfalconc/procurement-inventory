using System.Net;

using Procurement.Application.Common.Constants;

using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Receipts;

public class ReceiptFlowTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReceiptFlowTests(
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
    public async Task GetReceipts_ShouldReturnOk_WhenUserIsWarehouse()
    {
        AuthenticateAs(RoleConstants.Warehouse);


        var response = await _client.GetAsync(
            "/api/receipts");


        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }


    [Fact]
    public async Task GetReceiptById_ShouldReturnNotFound_WhenReceiptDoesNotExist()
    {
        AuthenticateAs(RoleConstants.Warehouse);


        var response = await _client.GetAsync(
            $"/api/receipts/{Guid.NewGuid()}");


        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    [Fact]
    public async Task GetReceipts_ShouldRejectPurchaserRole()
    {
        AuthenticateAs(RoleConstants.Purchaser);


        var response = await _client.GetAsync(
            "/api/receipts");


        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}