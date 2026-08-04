using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Procurement.Infrastructure.Persistence;

namespace Procurement.UnitTests.Common;

public class SqliteTestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public ProcurementDbContext Context { get; }

    public SqliteTestDatabase()
    {
        _connection = new SqliteConnection(
            "DataSource=:memory:");

        _connection.Open();

        var options = new DbContextOptionsBuilder<ProcurementDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new ProcurementDbContext(options);

        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}