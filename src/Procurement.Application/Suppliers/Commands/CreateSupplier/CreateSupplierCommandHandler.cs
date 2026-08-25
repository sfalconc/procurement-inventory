using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Procurement.Application.Common.Interfaces;
using Procurement.Application.Suppliers.DTOs;
using Procurement.Domain.Entities;

namespace Procurement.Application.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler
    : IRequestHandler<CreateSupplierCommand, SupplierResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateSupplierCommandHandler> _logger;

    public CreateSupplierCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateSupplierCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SupplierResponse> Handle(
        CreateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var supplierExists = await _context.Suppliers
            .AnyAsync(
                s => s.Code == request.Code ||
                     s.TaxId == request.TaxId,
                cancellationToken);

        if (supplierExists)
        {
            _logger.LogWarning(
                "SupplierCreationFailed {SupplierCode} {TaxId} Duplicate",
                request.Code,
                request.TaxId);

            throw new InvalidOperationException(
                "Supplier with the same Code or TaxId already exists.");
        }

        var supplier = new Supplier(
            request.Code,
            request.Name,
            request.TaxId,
            request.ContactName,
            request.Email,
            request.Phone);

        await _context.Suppliers.AddAsync(
            supplier,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "SupplierCreated {SupplierId} {SupplierCode}",
            supplier.Id,
            supplier.Code);

        return new SupplierResponse
        {
            Id = supplier.Id,
            Code = supplier.Code,
            Name = supplier.Name,
            TaxId = supplier.TaxId,
            ContactName = supplier.ContactName,
            Email = supplier.Email,
            Phone = supplier.Phone
        };
    }
}