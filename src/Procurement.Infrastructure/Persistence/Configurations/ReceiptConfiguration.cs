using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Persistence.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.ToTable("Receipts");

        builder.HasKey(r => r.Id);


        builder.Property(r => r.ReceivedDate)
            .IsRequired();


        builder.HasOne(r => r.PurchaseOrder)
            .WithMany()
            .HasForeignKey(r => r.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasMany(r => r.Lines)
            .WithOne(rl => rl.Receipt)
            .HasForeignKey(rl => rl.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}