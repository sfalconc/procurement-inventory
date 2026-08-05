using Microsoft.EntityFrameworkCore;
using Procurement.Application.Suppliers.Commands.UpdateSupplier;
using Procurement.Domain.Entities;
using Procurement.UnitTests.Common;

namespace Procurement.UnitTests.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly UpdateSupplierCommandHandler _handler;

    public UpdateSupplierCommandHandlerTests()
    {
        _database = new SqliteTestDatabase();

        _handler = new UpdateSupplierCommandHandler(
            _database.Context);
    }


    [Fact]
    public async Task Handle_ShouldUpdateSupplier_WhenSupplierExists()
    {
        // Arrange
        var supplier = new Supplier(
            "SUP-001",
            "Old Supplier",
            "20111111111",
            "John",
            "old@test.com",
            "999111111");


        _database.Context.Suppliers.Add(supplier);

        await _database.Context.SaveChangesAsync();


        var command = new UpdateSupplierCommand(
            supplier.Id,
            "SUP-002",
            "New Supplier",
            "20222222222",
            "Jane",
            "new@test.com",
            "999222222");


        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);


        // Assert
        var updatedSupplier = await _database.Context.Suppliers
            .FirstAsync(
                s => s.Id == supplier.Id);


        Assert.Equal(
            command.Code,
            updatedSupplier.Code);

        Assert.Equal(
            command.Name,
            updatedSupplier.Name);

        Assert.Equal(
            command.TaxId,
            updatedSupplier.TaxId);

        Assert.Equal(
            command.ContactName,
            updatedSupplier.ContactName);

        Assert.Equal(
            command.Email,
            updatedSupplier.Email);

        Assert.Equal(
            command.Phone,
            updatedSupplier.Phone);
    }


    [Fact]
    public async Task Handle_ShouldThrowException_WhenSupplierDoesNotExist()
    {
        // Arrange
        var command = new UpdateSupplierCommand(
            Guid.NewGuid(),
            "SUP-001",
            "Supplier",
            "20111111111",
            "John",
            "john@test.com",
            "999111111");


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


    [Fact]
    public async Task Handle_ShouldThrowException_WhenCodeOrTaxIdAlreadyExists()
    {
        // Arrange
        var supplier1 = new Supplier(
            "SUP-001",
            "Supplier One",
            "20111111111",
            null,
            null,
            null);


        var supplier2 = new Supplier(
            "SUP-002",
            "Supplier Two",
            "20222222222",
            null,
            null,
            null);


        _database.Context.Suppliers.AddRange(
            supplier1,
            supplier2);


        await _database.Context.SaveChangesAsync();


        var command = new UpdateSupplierCommand(
            supplier2.Id,
            "SUP-001",
            "Updated Supplier",
            "20111111111",
            null,
            null,
            null);


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await _handler.Handle(
                    command,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Another supplier with the same Code or TaxId already exists.",
            exception.Message);
    }


    public void Dispose()
    {
        _database.Dispose();
    }
}