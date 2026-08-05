using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;

namespace Procurement.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.Id,
                cancellationToken);


        if (product is null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }


        var categoryExists = await _context.Categories
            .AnyAsync(
                c => c.Id == request.CategoryId,
                cancellationToken);


        if (!categoryExists)
        {
            throw new InvalidOperationException(
                "Category does not exist.");
        }


        product.UpdateDetails(
            request.Code,
            request.Name,
            request.Description,
            request.CategoryId);


        await _context.SaveChangesAsync(
            cancellationToken);
    }
}