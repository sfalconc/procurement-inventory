using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.Id);


        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(150);


        builder.Property(s => s.TaxId)
            .IsRequired()
            .HasMaxLength(50);


        builder.HasIndex(s => s.TaxId)
            .IsUnique();
    }
}