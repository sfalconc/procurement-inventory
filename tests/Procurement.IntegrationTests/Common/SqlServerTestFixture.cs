using Xunit;

namespace Procurement.IntegrationTests.Common;

public class SqlServerTestFixture : IAsyncLifetime
{
    public SqlServerTestContainer Container { get; } = new();

    public string ConnectionString =>
        Container.ConnectionString;

    public Task InitializeAsync()
    {
        return Container.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return Container.DisposeAsync();
    }
}