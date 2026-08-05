using Procurement.Application.Suppliers.Queries.GetSuppliers;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.Suppliers.Queries.GetSuppliers;

public class GetSuppliersQueryHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly GetSuppliersQueryHandler _handler;

    public GetSuppliersQueryHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new GetSuppliersQueryHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldReturnSuppliers()
    {
        // Arrange
        var supplier1 = new Supplier(
            "SUP-001",
            "ACME Supplies",
            "20111111111",
            "John Doe",
            "john@acme.com",
            "999888777");


        var supplier2 = new Supplier(
            "SUP-002",
            "Contoso Ltd.",
            "20222222222",
            "Jane Smith",
            "jane@contoso.com",
            "999777666");


        _database.Context.Suppliers.AddRange(
            supplier1,
            supplier2);


        await _database.Context.SaveChangesAsync();


        var query = new GetSuppliersQuery();


        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);


        // Assert
        var suppliers = result.ToList();


        Assert.Equal(
            2,
            suppliers.Count);


        Assert.Contains(
            suppliers,
            s => s.Code == "SUP-001");


        Assert.Contains(
            suppliers,
            s => s.Code == "SUP-002");
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}