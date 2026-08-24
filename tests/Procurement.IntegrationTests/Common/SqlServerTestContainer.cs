using Testcontainers.MsSql;

namespace Procurement.IntegrationTests.Common;

public class SqlServerTestContainer : IAsyncLifetime
{
    private readonly MsSqlContainer _container =
        new MsSqlBuilder(
            "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
            .Build();

    public string ConnectionString =>
        _container.GetConnectionString();

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }
}