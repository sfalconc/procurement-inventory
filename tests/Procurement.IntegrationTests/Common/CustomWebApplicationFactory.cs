using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;

using Procurement.Infrastructure.Persistence;

namespace Procurement.IntegrationTests.Common;

public class CustomWebApplicationFactory 
    : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;


    protected override void ConfigureWebHost(
        Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<ProcurementDbContext>));


            if (descriptor != null)
            {
                services.Remove(descriptor);
            }


            _connection = new SqliteConnection(
                "DataSource=:memory:");

            _connection.Open();


            services.AddDbContext<ProcurementDbContext>(
                options =>
                {
                    options.UseSqlite(_connection);
                });


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                "Test",
                options =>
                {
                });


            var provider = services.BuildServiceProvider();


            using var scope = provider.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ProcurementDbContext>();

            context.Database.EnsureCreated();
        });
    }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _connection?.Dispose();
    }
}