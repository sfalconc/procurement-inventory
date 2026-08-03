using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Persistence.Configurations;

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");

        builder.HasKey(im => im.Id);

        builder.Property(im => im.Quantity)
            .IsRequired();

        builder.Property(im => im.Type)
            .IsRequired();

        builder.Property(im => im.Description)
            .HasMaxLength(250);


        builder.HasOne(im => im.Inventory)
            .WithMany()
            .HasForeignKey(im => im.InventoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}