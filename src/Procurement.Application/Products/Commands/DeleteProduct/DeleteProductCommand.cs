using MediatR;

namespace Procurement.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(
    Guid Id
) : IRequest;