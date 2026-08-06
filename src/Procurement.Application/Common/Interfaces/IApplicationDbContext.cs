using Microsoft.EntityFrameworkCore;
using Procurement.Domain.Entities;
using InventoryEntity = Procurement.Domain.Entities.Inventory;

namespace Procurement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Category> Categories { get; }

    DbSet<Supplier> Suppliers { get; }

    DbSet<InventoryEntity> Inventories { get; }

    DbSet<InventoryMovement> InventoryMovements { get; }

    DbSet<PurchaseOrder> PurchaseOrders { get; }

    DbSet<PurchaseOrderLine> PurchaseOrderLines { get; }

    DbSet<Receipt> Receipts { get; }

    DbSet<ReceiptLine> ReceiptLines { get; }
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}