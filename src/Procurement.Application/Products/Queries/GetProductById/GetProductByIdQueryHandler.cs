using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Products.DTOs;

namespace Procurement.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler 
    : IRequestHandler<GetProductByIdQuery, ProductResponse?>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<ProductResponse?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(
                p => p.Id == request.Id,
                cancellationToken);


        if (product is null)
        {
            return null;
        }


        return new ProductResponse
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name
        };
    }
}