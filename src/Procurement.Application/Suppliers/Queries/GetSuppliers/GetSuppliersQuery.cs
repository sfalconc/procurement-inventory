using MediatR;
using Procurement.Application.Suppliers.DTOs;

namespace Procurement.Application.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery
    : IRequest<IEnumerable<SupplierResponse>>;