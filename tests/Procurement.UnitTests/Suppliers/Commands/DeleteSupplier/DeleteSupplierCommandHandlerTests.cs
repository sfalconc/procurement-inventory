using Microsoft.EntityFrameworkCore;
using Procurement.Application.Suppliers.Commands.DeleteSupplier;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly DeleteSupplierCommandHandler _handler;

    public DeleteSupplierCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new DeleteSupplierCommandHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldSoftDeleteSupplier_WhenSupplierExists()
    {
        // Arrange
        var supplier = new Supplier(
            "SUP-001",
            "Supplier",
            "20111111111",
            null,
            null,
            null);


        _database.Context.Suppliers.Add(supplier);

        await _database.Context.SaveChangesAsync();


        var command = new DeleteSupplierCommand(
            supplier.Id);


        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);


        // Assert
        var supplierInDb = await _database.Context.Suppliers
            .IgnoreQueryFilters()
            .FirstAsync(
                s => s.Id == supplier.Id);


        Assert.NotNull(supplierInDb);

        Assert.False(
            supplierInDb.IsActive);
    }


    [Fact]
    public async Task Handle_ShouldThrowException_WhenSupplierDoesNotExist()
    {
        // Arrange
        var command = new DeleteSupplierCommand(
            Guid.NewGuid());


        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () =>
            {
                await _handler.Handle(
                    command,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Supplier not found.",
            exception.Message);
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}