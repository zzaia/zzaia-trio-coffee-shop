using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zzaia.CoffeeShop.Order.Domain.Entities;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for User.
/// </summary>
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configures the User entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder instance.</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "order");
        builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(u => u.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.HasIndex(u => u.Email)
            .HasDatabaseName("idx_users_email");
    }
}
