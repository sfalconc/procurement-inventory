using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Common;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Persistence;

public class ProcurementDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public ProcurementDbContext(
        DbContextOptions<ProcurementDbContext> options,
        ICurrentUserService currentUserService,
        IDomainEventDispatcher domainEventDispatcher)
        : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<Inventory> Inventories => Set<Inventory>();

    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();

    public DbSet<Receipt> Receipts => Set<Receipt>();

    public DbSet<ReceiptLine> ReceiptLines => Set<ReceiptLine>();

    public new DbSet<User> Users => Set<User>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();

        IReadOnlyCollection<BaseEntity> entitiesWithEvents =
            GetEntitiesWithDomainEvents();

        int result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync(
            entitiesWithEvents,
            cancellationToken);

        return result;
    }

    private IReadOnlyCollection<BaseEntity> GetEntitiesWithDomainEvents()
    {
        return ChangeTracker
            .Entries()
            .Select(entry => entry.Entity)
            .OfType<BaseEntity>()
            .Where(entity => entity.DomainEvents.Count > 0)
            .ToArray();
    }

    private Task DispatchDomainEventsAsync(
        IReadOnlyCollection<BaseEntity> entities,
        CancellationToken cancellationToken)
    {
        if (entities.Count == 0)
        {
            return Task.CompletedTask;
        }

        return _domainEventDispatcher.DispatchAsync(
            entities,
            cancellationToken);
    }

    private void UpdateAuditFields()
    {
        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry.Entity is User user)
            {
                if (entry.State == EntityState.Added)
                {
                    user.CreatedAt = DateTime.UtcNow;
                    user.CreatedBy = _currentUserService.UserId;
                }

                if (entry.State == EntityState.Modified)
                {
                    user.MarkAsUpdated(_currentUserService.UserId);
                }

                continue;
            }

            if (entry.Entity is not Domain.Common.BaseEntity entity)
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.CreatedBy = _currentUserService.UserId;
            }

            if (entry.State == EntityState.Modified)
            {
                entity.MarkAsUpdated(_currentUserService.UserId);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable("Roles");
        });

        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("UserTokens");
        });

        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProcurementDbContext).Assembly);
    }
}