using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Common;
using Procurement.Infrastructure.Identity;

namespace Procurement.Infrastructure.Persistence;

public class ProcurementDbContextFactory : IDesignTimeDbContextFactory<ProcurementDbContext>
{
    public ProcurementDbContext CreateDbContext(string[] args)
    {
        var apiProjectPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "../Procurement.Api"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");

        var optionsBuilder = new DbContextOptionsBuilder<ProcurementDbContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new ProcurementDbContext(
            optionsBuilder.Options,
            new DesignTimeCurrentUserService(),
            new DesignTimeDomainEventDispatcher());
            }

    private sealed class DesignTimeDomainEventDispatcher
        : IDomainEventDispatcher
    {
        public Task DispatchAsync(
            IReadOnlyCollection<BaseEntity> entities,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}