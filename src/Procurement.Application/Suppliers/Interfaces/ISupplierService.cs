using Procurement.Application.Suppliers.DTOs;

namespace Procurement.Application.Suppliers.Interfaces;

public interface ISupplierService
{
    Task<SupplierResponse> CreateAsync(
        CreateSupplierRequest request,
        CancellationToken cancellationToken);

    Task<SupplierResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IEnumerable<SupplierResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Guid id,
        UpdateSupplierRequest request,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}