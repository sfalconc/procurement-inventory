using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Procurement.Infrastructure.Persistence;

namespace Procurement.IntegrationTests.Common;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public CustomWebApplicationFactory(
        string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting(
            "JwtSettings:SecretKey",
            "TestJwtSigningKeyForDevelopmentOnly_1234567890");

        builder.ConfigureAppConfiguration(
            (_, config) =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["JwtSettings:Issuer"] =
                            "Procurement.Api",

                        ["JwtSettings:Audience"] =
                            "Procurement.Client",

                        ["JwtSettings:ExpirationMinutes"] =
                            "120"
                    });
            });

        builder.ConfigureServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(d =>
                    d.ServiceType ==
                    typeof(DbContextOptions<ProcurementDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ProcurementDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        _connectionString);
                });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    "TestOrBearer";

                options.DefaultChallengeScheme =
                    "TestOrBearer";
            })
            .AddPolicyScheme(
                "TestOrBearer",
                "Test or Bearer",
                options =>
                {
                    options.ForwardDefaultSelector =
                        context =>
                            context.Request.Headers
                                .ContainsKey("Authorization")
                                ? "Bearer"
                                : "Test";
                })
            .AddScheme<
                AuthenticationSchemeOptions,
                TestAuthenticationHandler>(
                "Test",
                _ => { });

            var serviceProvider =
                services.BuildServiceProvider();

            using var scope =
                serviceProvider.CreateScope();

            var context =
                scope.ServiceProvider
                    .GetRequiredService<ProcurementDbContext>();

            context.Database.Migrate();
        });
    }
}