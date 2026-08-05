using Microsoft.EntityFrameworkCore;
using Procurement.Application.Suppliers.Commands.CreateSupplier;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly CreateSupplierCommandHandler _handler;

    public CreateSupplierCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new CreateSupplierCommandHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldCreateSupplier_WhenRequestIsValid()
    {
        // Arrange
        var command = new CreateSupplierCommand(
            "SUP-001",
            "ACME Supplies",
            "20123456789",
            "John Doe",
            "john@acme.com",
            "999888777");


        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);


        // Assert
        Assert.NotEqual(
            Guid.Empty,
            result.Id);

        Assert.Equal(
            command.Code,
            result.Code);

        Assert.Equal(
            command.Name,
            result.Name);

        Assert.Equal(
            command.TaxId,
            result.TaxId);


        var supplierInDb = await _database.Context.Suppliers
            .FirstOrDefaultAsync(
                s => s.Id == result.Id);


        Assert.NotNull(supplierInDb);

        Assert.Equal(
            command.Name,
            supplierInDb.Name);
    }


    [Fact]
    public async Task Handle_ShouldThrowException_WhenCodeAlreadyExists()
    {
        // Arrange
        var supplier = new Supplier(
            "SUP-001",
            "Existing Supplier",
            "20111111111",
            null,
            null,
            null);


        _database.Context.Suppliers.Add(supplier);

        await _database.Context.SaveChangesAsync();


        var command = new CreateSupplierCommand(
            "SUP-001",
            "New Supplier",
            "20222222222",
            "John Doe",
            "john@test.com",
            "999999999");


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await _handler.Handle(
                    command,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Supplier with the same Code or TaxId already exists.",
            exception.Message);


        Assert.Equal(
            1,
            await _database.Context.Suppliers.CountAsync());
    }


    [Fact]
    public async Task Handle_ShouldThrowException_WhenTaxIdAlreadyExists()
    {
        // Arrange
        var supplier = new Supplier(
            "SUP-001",
            "Existing Supplier",
            "20123456789",
            null,
            null,
            null);


        _database.Context.Suppliers.Add(supplier);

        await _database.Context.SaveChangesAsync();


        var command = new CreateSupplierCommand(
            "SUP-002",
            "New Supplier",
            "20123456789",
            "John Doe",
            "john@test.com",
            "999999999");


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await _handler.Handle(
                    command,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Supplier with the same Code or TaxId already exists.",
            exception.Message);


        Assert.Equal(
            1,
            await _database.Context.Suppliers.CountAsync());
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}