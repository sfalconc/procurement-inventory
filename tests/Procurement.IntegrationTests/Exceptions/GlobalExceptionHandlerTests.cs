using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Procurement.IntegrationTests.Common;

namespace Procurement.IntegrationTests.Exceptions;

public class GlobalExceptionHandlerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GlobalExceptionHandlerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Post_WithInvalidData_ReturnsStructuredValidationProblemDetails()
    {
        var invalidPayload = new { Email = string.Empty, Password = string.Empty };

        var response = await _client.PostAsJsonAsync("/api/auth/login", invalidPayload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.True(problemDetails.Errors.Any());
    }

    [Fact]
    public async Task Get_WithUnhandledException_DoesNotExposeExceptionMessage()
    {
        var response = await _client.GetAsync("/api/force-error");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal("https://httpstatuses.com/500", problemDetails.Type);
        Assert.Equal("An unexpected error occurred.", problemDetails.Title);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("An unexpected error occurred while processing your request.", problemDetails.Detail);
    }
}