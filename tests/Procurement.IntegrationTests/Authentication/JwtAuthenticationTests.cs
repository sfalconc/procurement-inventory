using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Procurement.Domain.Entities;
using Procurement.Domain.Enums;
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

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<User>>();
        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var user = new User(username, string.Empty, role);
        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    "; ",
                    createResult.Errors.Select(error => error.Description)));
        }

        var roleName = role.ToString();
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(
                    new IdentityRole<Guid>(roleName));

            if (!roleResult.Succeeded)
            {
                    throw new InvalidOperationException(
                        string.Join(
                            "; ",
                            roleResult.Errors.Select(error => error.Description)));
            }
        }

        var addRoleResult = await userManager.AddToRoleAsync(user, roleName);
        if (!addRoleResult.Succeeded)
        {
            throw new InvalidOperationException(
                    string.Join(
                        "; ",
                        addRoleResult.Errors.Select(error => error.Description)));
        }

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