using Procurement.Application.Suppliers.Services;
using Procurement.UnitTests.Common;
using Procurement.Domain.Entities;
using Procurement.Application.Suppliers.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Procurement.UnitTests.Suppliers.Services;

public class SupplierServiceTests : IDisposable
{
    private readonly SqliteTestDatabase _database;
    private readonly SupplierService _service;

    public SupplierServiceTests()
    {
        _database = new SqliteTestDatabase();

        _service = new SupplierService(
            _database.Context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateSupplier_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateSupplierRequest
        {
            Code = "SUP-001",
            Name = "ACME Supplies",
            TaxId = "20123456789",
            ContactName = "John Doe",
            Email = "john@acme.com",
            Phone = "999888777"
        };


        // Act
        var result = await _service.CreateAsync(
            request,
            CancellationToken.None);


        // Assert
        Assert.NotEqual(
            Guid.Empty,
            result.Id);

        Assert.Equal(
            request.Code,
            result.Code);

        Assert.Equal(
            request.Name,
            result.Name);

        Assert.Equal(
            request.TaxId,
            result.TaxId);


        var supplierInDb = await _database.Context.Suppliers
            .FirstOrDefaultAsync(
                s => s.Id == result.Id);

        Assert.NotNull(supplierInDb);

        Assert.Equal(
            request.Name,
            supplierInDb.Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenCodeAlreadyExists()
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


        var request = new CreateSupplierRequest
        {
            Code = "SUP-001",
            Name = "New Supplier",
            TaxId = "20222222222",
            ContactName = "John Doe",
            Email = "john@test.com",
            Phone = "999999999"
        };


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await _service.CreateAsync(
                    request,
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
    public async Task CreateAsync_ShouldThrowException_WhenTaxIdAlreadyExists()
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


        var request = new CreateSupplierRequest
        {
            Code = "SUP-002",
            Name = "New Supplier",
            TaxId = "20123456789",
            ContactName = "John Doe",
            Email = "john@test.com",
            Phone = "999999999"
        };


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await _service.CreateAsync(
                    request,
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
    public async Task GetByIdAsync_ShouldReturnSupplier_WhenSupplierExists()
    {
        // Arrange
        var supplier = new Supplier(
            "SUP-001",
            "ACME Supplies",
            "20123456789",
            "John Doe",
            "john@acme.com",
            "999888777");

        _database.Context.Suppliers.Add(supplier);

        await _database.Context.SaveChangesAsync();


        // Act
        var result = await _service.GetByIdAsync(
            supplier.Id,
            CancellationToken.None);


        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            supplier.Id,
            result.Id);

        Assert.Equal(
            supplier.Code,
            result.Code);

        Assert.Equal(
            supplier.Name,
            result.Name);

        Assert.Equal(
            supplier.TaxId,
            result.TaxId);

        Assert.Equal(
            supplier.ContactName,
            result.ContactName);

        Assert.Equal(
            supplier.Email,
            result.Email);

        Assert.Equal(
            supplier.Phone,
            result.Phone);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenSupplierDoesNotExist()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();


        // Act
        var result = await _service.GetByIdAsync(
            nonExistingId,
            CancellationToken.None);


        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnSuppliers()
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


        // Act
        var result = await _service.GetAllAsync(
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

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSupplier_WhenSupplierExists()
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


        var request = new UpdateSupplierRequest
        {
            Code = "SUP-002",
            Name = "New Supplier",
            TaxId = "20222222222",
            ContactName = "Jane",
            Email = "new@test.com",
            Phone = "999222222"
        };


        // Act
        await _service.UpdateAsync(
            supplier.Id,
            request,
            CancellationToken.None);


        // Assert
        var updatedSupplier = await _database.Context.Suppliers
            .FirstAsync(s => s.Id == supplier.Id);


        Assert.Equal(
            request.Code,
            updatedSupplier.Code);

        Assert.Equal(
            request.Name,
            updatedSupplier.Name);

        Assert.Equal(
            request.TaxId,
            updatedSupplier.TaxId);

        Assert.Equal(
            request.ContactName,
            updatedSupplier.ContactName);

        Assert.Equal(
            request.Email,
            updatedSupplier.Email);

        Assert.Equal(
            request.Phone,
            updatedSupplier.Phone);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenSupplierDoesNotExist()
    {
        // Arrange
        var request = new UpdateSupplierRequest
        {
            Code = "SUP-001",
            Name = "Supplier",
            TaxId = "20111111111",
            ContactName = "John",
            Email = "john@test.com",
            Phone = "999111111"
        };


        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () =>
            {
                await _service.UpdateAsync(
                    Guid.NewGuid(),
                    request,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Supplier not found.",
            exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenCodeOrTaxIdAlreadyExists()
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


        var request = new UpdateSupplierRequest
        {
            Code = "SUP-001",
            Name = "Updated Supplier",
            TaxId = "20111111111",
            ContactName = null,
            Email = null,
            Phone = null
        };


        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await _service.UpdateAsync(
                    supplier2.Id,
                    request,
                    CancellationToken.None);
            });


        Assert.Equal(
            "Another supplier with the same Code or TaxId already exists.",
            exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteSupplier_WhenSupplierExists()
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


        // Act
        await _service.DeleteAsync(
            supplier.Id,
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
    public async Task DeleteAsync_ShouldThrowException_WhenSupplierDoesNotExist()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();


        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            async () =>
            {
                await _service.DeleteAsync(
                    nonExistingId,
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