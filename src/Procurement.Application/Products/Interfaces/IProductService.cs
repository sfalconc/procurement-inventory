using Procurement.Application.Products.DTOs;

namespace Procurement.Application.Products.Interfaces;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IEnumerable<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}