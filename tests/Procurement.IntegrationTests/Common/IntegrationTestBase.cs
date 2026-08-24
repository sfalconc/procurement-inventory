namespace Procurement.IntegrationTests.Common;

public abstract class IntegrationTestBase
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(
        SqlServerTestFixture fixture)
    {
        Factory = new CustomWebApplicationFactory(
            fixture.ConnectionString);

        Client = Factory.CreateClient();
    }
}