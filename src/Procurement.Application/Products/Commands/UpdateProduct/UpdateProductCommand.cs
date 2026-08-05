using MediatR;

namespace Procurement.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    Guid CategoryId
) : IRequest;