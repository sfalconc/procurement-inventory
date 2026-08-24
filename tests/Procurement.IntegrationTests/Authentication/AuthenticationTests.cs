using System.Net;
using System.Net.Http.Json;

using Microsoft.Extensions.DependencyInjection;

using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Entities;
using Procurement.Domain.Enums;
using Procurement.Infrastructure.Persistence;

using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Authentication;
[Collection("Integration Tests")]
public class AuthenticationTests
    : IntegrationTestBase
{
    public AuthenticationTests(
        SqlServerTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        await SeedUserAsync(
            "test-purchaser",
            "Password123!",
            UserRole.Purchaser);

        var response = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Username = "test-purchaser",
                Password = "Password123!"
            });

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Username = "does-not-exist",
                Password = "Password123!"
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        await SeedUserAsync(
            "test-user",
            "Password123!",
            UserRole.Purchaser);

        var response = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Username = "test-user",
                Password = "WrongPassword!"
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    private async Task SeedUserAsync(
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

        var user = new User(
            username,
            passwordHasher.HashPassword(password),
            role);

        context.Users.Add(user);

        await context.SaveChangesAsync();
    }

    private record LoginResponse(string Token);
}