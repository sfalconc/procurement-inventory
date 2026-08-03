using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Persistence.Configurations;

public class ReceiptLineConfiguration : IEntityTypeConfiguration<ReceiptLine>
{
    public void Configure(EntityTypeBuilder<ReceiptLine> builder)
    {
        builder.ToTable("ReceiptLines");

        builder.HasKey(rl => rl.Id);


        builder.Property(rl => rl.QuantityReceived)
            .IsRequired();


        builder.HasOne(rl => rl.PurchaseOrderLine)
            .WithMany(pol => pol.ReceiptLines)
            .HasForeignKey(rl => rl.PurchaseOrderLineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}