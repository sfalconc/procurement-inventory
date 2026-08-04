using Microsoft.EntityFrameworkCore;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Products.DTOs;
using Procurement.Application.Products.Interfaces;
using Procurement.Domain.Entities;

namespace Procurement.Application.Products.Services;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _context;

    public ProductService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
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

    public async Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(
                p => p.Id == id,
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

    public async Task<IEnumerable<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return products.Select(product => new ProductResponse
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name
        });
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == id,
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

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == id,
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