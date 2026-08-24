using Xunit;

namespace Procurement.IntegrationTests.Common;

[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection
    : ICollectionFixture<SqlServerTestFixture>
{
}