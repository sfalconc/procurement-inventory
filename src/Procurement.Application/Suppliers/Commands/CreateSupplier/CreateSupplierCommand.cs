using MediatR;
using Procurement.Application.Suppliers.DTOs;

namespace Procurement.Application.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand(
    string Code,
    string Name,
    string TaxId,
    string? ContactName,
    string? Email,
    string? Phone
) : IRequest<SupplierResponse>;