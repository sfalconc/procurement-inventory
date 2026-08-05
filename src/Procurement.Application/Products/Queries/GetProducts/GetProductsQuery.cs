using MediatR;
using Procurement.Application.Products.DTOs;

namespace Procurement.Application.Products.Queries.GetProducts;

public record GetProductsQuery
    : IRequest<IEnumerable<ProductResponse>>;