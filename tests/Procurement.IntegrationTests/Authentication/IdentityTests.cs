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
public class IdentityTests : IntegrationTestBase
{
    public IdentityTests(SqlServerTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UserManager_ShouldCreateUserWithIdentityHashAndNormalizedName()
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();
        var username = $"identity-user-{Guid.NewGuid():N}";
        var password = "IdentityPassword123";

        var user = new User(username, string.Empty, UserRole.Purchaser);
        var result = await userManager.CreateAsync(user, password);

        Assert.True(result.Succeeded);
        Assert.Equal(username.ToUpperInvariant(), user.NormalizedUserName);
        Assert.NotNull(user.PasswordHash);
        Assert.True(await userManager.CheckPasswordAsync(user, password));
    }

    [Theory]
    [InlineData("short")]
    [InlineData("lowercasepassword")]
    [InlineData("UPPERCASEPASSWORD")]
    [InlineData("NoDigitsPassword")]
    public async Task UserManager_ShouldRejectPasswordThatViolatesPolicy(
        string password)
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();
        var user = new User(
            $"invalid-password-{Guid.NewGuid():N}",
            string.Empty,
            UserRole.Purchaser);

        var result = await userManager.CreateAsync(user, password);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, error => error.Code == "PasswordRequiresDigit" ||
                                                error.Code == "PasswordRequiresUpper" ||
                                                error.Code == "PasswordRequiresLower" ||
                                                error.Code == "PasswordTooShort");
    }

    [Fact]
    public async Task IdentityRole_ShouldBeAssignableAndAuthorizeExistingEndpoint()
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var username = $"identity-role-{Guid.NewGuid():N}";
        var roleName = "Purchaser";
        var user = new User(username, string.Empty, UserRole.Purchaser);

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole<Guid>(roleName));
            Assert.True(roleResult.Succeeded);
        }

        var createResult = await userManager.CreateAsync(
            user,
            "IdentityPassword123");
        Assert.True(createResult.Succeeded);

        var addRoleResult = await userManager.AddToRoleAsync(user, roleName);
        Assert.True(addRoleResult.Succeeded);
        Assert.Contains(roleName, await userManager.GetRolesAsync(user));

        var loginResponse = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Username = username,
                Password = "IdentityPassword123"
            });
        loginResponse.EnsureSuccessStatusCode();
        var login = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponse>();

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login!.Token);

        var response = await Client.GetAsync("/api/purchaseorders");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private sealed record LoginResponse(string Token);
}
