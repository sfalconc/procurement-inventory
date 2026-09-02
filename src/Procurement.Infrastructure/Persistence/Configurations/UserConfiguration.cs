using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(
        EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName)
            .HasColumnName("Username")
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasColumnName("CreatedBy");

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.UpdatedBy);
    }
}