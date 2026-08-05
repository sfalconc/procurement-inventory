using MediatR;

namespace Procurement.Application.Suppliers.Commands.DeleteSupplier;

public record DeleteSupplierCommand(
    Guid Id
) : IRequest;