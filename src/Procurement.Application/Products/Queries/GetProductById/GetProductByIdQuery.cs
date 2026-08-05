using MediatR;
using Procurement.Application.Products.DTOs;

namespace Procurement.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(
    Guid Id
) : IRequest<ProductResponse?>;