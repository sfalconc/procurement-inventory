using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

using Procurement.Domain.Entities;
using Procurement.Domain.Enums;
using Procurement.Infrastructure.Identity;

namespace Procurement.UnitTests.Authentication;

public class JwtTokenGeneratorTests
{
    [Fact]
    public async Task GenerateTokenAsync_ShouldCreateToken_WhenJwtSecretIsConfigured()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "TestJwtSigningKeyForDevelopmentOnly_1234567890",
                ["JwtSettings:Issuer"] = "Procurement.Api",
                ["JwtSettings:Audience"] = "Procurement.Client",
                ["JwtSettings:ExpirationMinutes"] = "120"
            })
            .Build();

        var userManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var user = new User("jwt-user", "password-hash", UserRole.Purchaser);
        userManager
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new[] { UserRole.Purchaser.ToString() });

        var generator = new JwtTokenGenerator(configuration, userManager.Object);

        var token = await generator.GenerateTokenAsync(user);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldThrow_WhenJwtSecretIsTooShort()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "short-key",
                ["JwtSettings:Issuer"] = "Procurement.Api",
                ["JwtSettings:Audience"] = "Procurement.Client",
                ["JwtSettings:ExpirationMinutes"] = "120"
            })
            .Build();

        var userManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var user = new User("jwt-user", "password-hash", UserRole.Purchaser);
        userManager
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new[] { UserRole.Purchaser.ToString() });

        var generator = new JwtTokenGenerator(configuration, userManager.Object);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => generator.GenerateTokenAsync(user));

        Assert.Equal(
            "JWT SecretKey must be at least 32 characters long.",
            exception.Message);
    }

    [Theory]
    [InlineData(null, "JWT SecretKey is not configured.")]
    [InlineData("", "JWT SecretKey is not configured.")]
    [InlineData("   ", "JWT SecretKey is not configured.")]
    public async Task GenerateTokenAsync_ShouldThrow_WhenJwtSecretIsMissingOrBlank(
        string? secretKey,
        string expectedMessage)
    {
        var generator = CreateGenerator(
            secretKey,
            "Procurement.Api",
            "Procurement.Client",
            "120");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => generator.GenerateTokenAsync(CreateUser()));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData(null, "JWT Issuer is not configured.")]
    [InlineData("", "JWT Issuer is not configured.")]
    [InlineData("   ", "JWT Issuer is not configured.")]
    public async Task GenerateTokenAsync_ShouldThrow_WhenIssuerIsMissingOrBlank(
        string? issuer,
        string expectedMessage)
    {
        var generator = CreateGenerator(
            "TestJwtSigningKeyForDevelopmentOnly_1234567890",
            issuer,
            "Procurement.Client",
            "120");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => generator.GenerateTokenAsync(CreateUser()));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData(null, "JWT Audience is not configured.")]
    [InlineData("", "JWT Audience is not configured.")]
    [InlineData("   ", "JWT Audience is not configured.")]
    public async Task GenerateTokenAsync_ShouldThrow_WhenAudienceIsMissingOrBlank(
        string? audience,
        string expectedMessage)
    {
        var generator = CreateGenerator(
            "TestJwtSigningKeyForDevelopmentOnly_1234567890",
            "Procurement.Api",
            audience,
            "120");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => generator.GenerateTokenAsync(CreateUser()));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData("not-an-integer")]
    [InlineData("0")]
    [InlineData("-1")]
    public async Task GenerateTokenAsync_ShouldThrow_WhenExpirationIsInvalid(
        string expirationMinutes)
    {
        var generator = CreateGenerator(
            "TestJwtSigningKeyForDevelopmentOnly_1234567890",
            "Procurement.Api",
            "Procurement.Client",
            expirationMinutes);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => generator.GenerateTokenAsync(CreateUser()));

        Assert.Equal(
            "JWT ExpirationMinutes must be a positive integer.",
            exception.Message);
    }

    private static JwtTokenGenerator CreateGenerator(
        string? secretKey,
        string? issuer,
        string? audience,
        string expirationMinutes)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = secretKey,
                ["JwtSettings:Issuer"] = issuer,
                ["JwtSettings:Audience"] = audience,
                ["JwtSettings:ExpirationMinutes"] = expirationMinutes
            })
            .Build();

        var userManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        userManager
            .Setup(x => x.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new[] { UserRole.Purchaser.ToString() });

        return new JwtTokenGenerator(configuration, userManager.Object);
    }

    private static User CreateUser()
    {
        return new User("jwt-user", "password-hash", UserRole.Purchaser);
    }
}
