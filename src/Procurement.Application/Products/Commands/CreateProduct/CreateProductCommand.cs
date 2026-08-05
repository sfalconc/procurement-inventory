using MediatR;
using Procurement.Application.Products.DTOs;

namespace Procurement.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Code,
    string Name,
    string? Description,
    Guid CategoryId
) : IRequest<ProductResponse>;