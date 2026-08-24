using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Exceptions;

[Collection("Integration Tests")]
public class GlobalExceptionHandlerTests
    : IntegrationTestBase
{
    public GlobalExceptionHandlerTests(
        SqlServerTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Post_WithInvalidData_ReturnsStructuredValidationProblemDetails()
    {
        var invalidPayload = new
        {
            Email = string.Empty,
            Password = string.Empty
        };

        var response = await Client.PostAsJsonAsync(
            "/api/auth/login",
            invalidPayload);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        var problemDetails =
            await response.Content
                .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal(
            StatusCodes.Status400BadRequest,
            problemDetails.Status);

        Assert.True(problemDetails.Errors.Any());
    }

    [Fact]
    public async Task Get_WithUnhandledException_DoesNotExposeExceptionMessage()
    {
        var response = await Client.GetAsync(
            "/api/force-error");

        Assert.Equal(
            HttpStatusCode.InternalServerError,
            response.StatusCode);

        var problemDetails =
            await response.Content
                .ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);

        Assert.Equal(
            "https://httpstatuses.com/500",
            problemDetails.Type);

        Assert.Equal(
            "An unexpected error occurred.",
            problemDetails.Title);

        Assert.Equal(
            StatusCodes.Status500InternalServerError,
            problemDetails.Status);

        Assert.Equal(
            "An unexpected error occurred while processing your request.",
            problemDetails.Detail);
    }
}