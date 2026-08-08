using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Procurement.Infrastructure.Persistence;

namespace Procurement.IntegrationTests.Common;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _databasePath;

    public CustomWebApplicationFactory()
    {
        _databasePath = Path.Combine(
            Path.GetTempPath(),
            $"ProcurementTests_{Guid.NewGuid():N}.db");
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

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
                    options.UseSqlite(
                        $"Data Source={_databasePath}");
                });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<
                AuthenticationSchemeOptions,
                TestAuthenticationHandler>(
                "Test",
                options =>
                {
                });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ProcurementDbContext>();

            context.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}