using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;

namespace Procurement.Application.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandHandler
    : IRequestHandler<UpdateSupplierCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSupplierCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task Handle(
        UpdateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(
                s => s.Id == request.Id,
                cancellationToken);


        if (supplier is null)
        {
            throw new KeyNotFoundException(
                "Supplier not found.");
        }


        var supplierExists = await _context.Suppliers
            .AnyAsync(
                s => (s.Code == request.Code || s.TaxId == request.TaxId)
                    && s.Id != request.Id,
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
}