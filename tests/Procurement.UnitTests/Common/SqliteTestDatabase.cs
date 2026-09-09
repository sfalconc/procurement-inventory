using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Entities;
using Procurement.Infrastructure.Persistence;

namespace Procurement.UnitTests.Common;

public class SqliteTestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public ProcurementDbContext Context { get; }
    public TestDomainEventDispatcher DomainEventDispatcher { get; }

    public SqliteTestDatabase()
    {
        _connection = new SqliteConnection(
            "DataSource=:memory:");

        _connection.Open();

        var options = new DbContextOptionsBuilder<ProcurementDbContext>()
            .UseSqlite(_connection)
            .Options;

        DomainEventDispatcher = new TestDomainEventDispatcher();
        Context = new SqliteProcurementDbContext(
            options,
            new TestCurrentUserService(),
            DomainEventDispatcher);

        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }

    private sealed class SqliteProcurementDbContext : ProcurementDbContext
    {
        public SqliteProcurementDbContext(
            DbContextOptions<ProcurementDbContext> options,
            ICurrentUserService currentUserService,
            IDomainEventDispatcher domainEventDispatcher)
            : base(options, currentUserService, domainEventDispatcher)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (Database.IsSqlite())
            {
                modelBuilder.Entity<global::Procurement.Domain.Entities.Inventory>()
                    .Property(i => i.RowVersion)
                    .HasDefaultValue(new byte[] { 0 })
                    .ValueGeneratedOnAddOrUpdate();
            }
        }
    }
}
