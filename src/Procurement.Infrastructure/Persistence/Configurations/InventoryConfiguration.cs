using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories", table =>
        {
            table.HasCheckConstraint(
                "CK_Inventory_CurrentQuantity",
                "[CurrentQuantity] >= 0"
            );
        });

        builder.HasKey(i => i.Id);

        builder.Property(i => i.CurrentQuantity)
            .IsRequired();

        builder.Property(i => i.AverageUnitCost)
            .HasPrecision(18, 2);

        builder.HasOne(i => i.Product)
            .WithOne(p => p.Inventory)
            .HasForeignKey<Inventory>(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}