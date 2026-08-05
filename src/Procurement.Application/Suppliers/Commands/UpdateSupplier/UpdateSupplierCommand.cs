using MediatR;

namespace Procurement.Application.Suppliers.Commands.UpdateSupplier;

public record UpdateSupplierCommand(
    Guid Id,
    string Code,
    string Name,
    string TaxId,
    string? ContactName,
    string? Email,
    string? Phone
) : IRequest;