using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.Extensions.DependencyInjection;

using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Entities;
using Procurement.Domain.Enums;
using Procurement.Infrastructure.Persistence;

using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Authentication;

[Collection("Integration Tests")]
public class JwtAuthenticationTests : IntegrationTestBase
{
    public JwtAuthenticationTests(
        SqlServerTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task ProtectedEndpoint_ShouldReturnUnauthorized_WithoutJwt()
    {
        var response = await Client.GetAsync(
            "/api/purchaseorders");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_ShouldAllowAccess_WithValidJwtAndAuthorizedRole()
    {
        var token = await LoginAsync(
            "jwt-purchaser",
            "Password123!",
            UserRole.Purchaser);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response = await Client.GetAsync(
            "/api/purchaseorders");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_ShouldReturnForbidden_WithValidJwtAndUnauthorizedRole()
    {
        var token = await LoginAsync(
            "jwt-warehouse",
            "Password123!",
            UserRole.Warehouse);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response = await Client.GetAsync(
            "/api/purchaseorders");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    private async Task<string> LoginAsync(
        string username,
        string password,
        UserRole role)
    {
        using var scope =
            Factory.Services.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<ProcurementDbContext>();

        var passwordHasher =
            scope.ServiceProvider
                .GetRequiredService<IPasswordHasher>();

        context.Users.Add(
            new User(
                username,
                passwordHasher.HashPassword(password),
                role));

        await context.SaveChangesAsync();

        var response = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Username = username,
                Password = password
            });

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        return result!.Token;
    }

    private record LoginResponse(string Token);
}