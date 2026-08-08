using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Procurement.IntegrationTests.Common;

public class TestAuthenticationHandler 
    : AuthenticationHandler<AuthenticationSchemeOptions>
{

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }


    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(
                "X-Test-Role",
                out var roleHeader))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        var role = roleHeader.ToString();

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                "test-user-id"),

            new Claim(
                ClaimTypes.Role,
                role)
        };

        var identity = new ClaimsIdentity(
            claims,
            "Test");

        var principal = new ClaimsPrincipal(
            identity);

        var ticket = new AuthenticationTicket(
            principal,
            "Test");

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}