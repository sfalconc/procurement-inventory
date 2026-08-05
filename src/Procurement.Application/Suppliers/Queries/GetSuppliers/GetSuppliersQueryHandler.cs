using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Suppliers.DTOs;

namespace Procurement.Application.Suppliers.Queries.GetSuppliers;

public class GetSuppliersQueryHandler
    : IRequestHandler<GetSuppliersQuery, IEnumerable<SupplierResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetSuppliersQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<SupplierResponse>> Handle(
        GetSuppliersQuery request,
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
}