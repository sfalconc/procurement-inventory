using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;

namespace Procurement.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task Handle(
        DeleteProductCommand request,
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


        product.Delete();

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}