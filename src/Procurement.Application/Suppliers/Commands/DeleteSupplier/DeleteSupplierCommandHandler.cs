using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;

namespace Procurement.Application.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierCommandHandler
    : IRequestHandler<DeleteSupplierCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteSupplierCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task Handle(
        DeleteSupplierCommand request,
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


        supplier.Delete();

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}