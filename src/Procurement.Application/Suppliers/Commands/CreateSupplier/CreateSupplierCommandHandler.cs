using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Suppliers.DTOs;
using Procurement.Domain.Entities;

namespace Procurement.Application.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler
    : IRequestHandler<CreateSupplierCommand, SupplierResponse>
{
    private readonly IApplicationDbContext _context;

    public CreateSupplierCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<SupplierResponse> Handle(
        CreateSupplierCommand request,
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
}