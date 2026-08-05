using MediatR;
using Procurement.Application.Suppliers.DTOs;

namespace Procurement.Application.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(
    Guid Id
) : IRequest<SupplierResponse?>;