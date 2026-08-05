using MediatR;
using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Products.DTOs;
using Procurement.Domain.Entities;

namespace Procurement.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler 
    : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<ProductResponse> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(
                c => c.Id == request.CategoryId,
                cancellationToken);


        if (!categoryExists)
        {
            throw new InvalidOperationException(
                "Category does not exist.");
        }


        var product = new Product(
            request.Code,
            request.Name,
            request.Description,
            request.CategoryId);


        await _context.Products.AddAsync(
            product,
            cancellationToken);


        await _context.SaveChangesAsync(
            cancellationToken);


        return new ProductResponse
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId
        };
    }
}