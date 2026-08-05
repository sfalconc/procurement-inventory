using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Suppliers.DTOs;
using Procurement.Application.Suppliers.Interfaces;
using Procurement.Domain.Entities;

namespace Procurement.Application.Suppliers.Services;

public class SupplierService : ISupplierService
{
    private readonly IApplicationDbContext _context;

    public SupplierService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierResponse> CreateAsync(
        CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var supplierExists = await _context.Suppliers
            .AnyAsync(
                s => s.Code == request.Code || s.TaxId == request.TaxId,
                cancellationToken);

        if (supplierExists)
        {
            throw new InvalidOperationException(
                "Supplier with the same Code or TaxId already exists.");
        }

        var supplier = new Supplier(
            request.Code,
            request.Name,
            request.TaxId,
            request.ContactName,
            request.Email,
            request.Phone);

        await _context.Suppliers.AddAsync(
            supplier,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return new SupplierResponse
        {
            Id = supplier.Id,
            Code = supplier.Code,
            Name = supplier.Name,
            TaxId = supplier.TaxId,
            ContactName = supplier.ContactName,
            Email = supplier.Email,
            Phone = supplier.Phone
        };
    }    

    public async Task<SupplierResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(
                s => s.Id == id,
                cancellationToken);

        if (supplier is null)
        {
            return null;
        }

        return new SupplierResponse
        {
            Id = supplier.Id,
            Code = supplier.Code,
            Name = supplier.Name,
            TaxId = supplier.TaxId,
            ContactName = supplier.ContactName,
            Email = supplier.Email,
            Phone = supplier.Phone
        };
    }    

    public async Task<IEnumerable<SupplierResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var suppliers = await _context.Suppliers
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return suppliers.Select(supplier => new SupplierResponse
        {
            Id = supplier.Id,
            Code = supplier.Code,
            Name = supplier.Name,
            TaxId = supplier.TaxId,
            ContactName = supplier.ContactName,
            Email = supplier.Email,
            Phone = supplier.Phone
        });
    }


    public async Task UpdateAsync(
        Guid id,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(
                s => s.Id == id,
                cancellationToken);

        if (supplier is null)
        {
            throw new KeyNotFoundException(
                "Supplier not found.");
        }

        var supplierExists = await _context.Suppliers
            .AnyAsync(
                s => (s.Code == request.Code || s.TaxId == request.TaxId)
                    && s.Id != id,
                cancellationToken);

        if (supplierExists)
        {
            throw new InvalidOperationException(
                "Another supplier with the same Code or TaxId already exists.");
        }

        supplier.UpdateDetails(
            request.Code,
            request.Name,
            request.TaxId,
            request.ContactName,
            request.Email,
            request.Phone);

        await _context.SaveChangesAsync(
            cancellationToken);
    }


    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(
                s => s.Id == id,
                cancellationToken);

        if (supplier is null)
        {
            throw new KeyNotFoundException(
                "Supplier not found.");
        }

        supplier.Delete();

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}