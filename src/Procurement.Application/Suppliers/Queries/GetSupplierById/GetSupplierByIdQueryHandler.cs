using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Suppliers.DTOs;

namespace Procurement.Application.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler
    : IRequestHandler<GetSupplierByIdQuery, SupplierResponse?>
{
    private readonly IApplicationDbContext _context;

    public GetSupplierByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<SupplierResponse?> Handle(
        GetSupplierByIdQuery request,
        CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(
                s => s.Id == request.Id,
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
}