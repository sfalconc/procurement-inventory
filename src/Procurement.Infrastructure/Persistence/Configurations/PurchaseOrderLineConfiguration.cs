using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Persistence.Configurations;

public class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.ToTable("PurchaseOrderLines");

        builder.HasKey(pol => pol.Id);


        builder.Property(pol => pol.Quantity)
            .IsRequired();


        builder.Property(pol => pol.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();


        builder.HasOne(pol => pol.Product)
            .WithMany()
            .HasForeignKey(pol => pol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}