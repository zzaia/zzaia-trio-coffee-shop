using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zzaia.CoffeeShop.Order.Domain.Entities;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for Product.
/// </summary>
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Configures the Product entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder instance.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", "order");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("product_id")
            .ValueGeneratedNever();
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(p => p.Category)
            .HasColumnName("category")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(p => p.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(500);
        builder.Property(p => p.IsAvailable)
            .HasColumnName("is_available")
            .IsRequired();
        builder.OwnsOne(p => p.BasePrice, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("base_price_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            price.Property(m => m.Currency)
                .HasColumnName("base_price_currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        builder.HasMany(p => p.Variations)
            .WithOne()
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(p => p.Variations)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(p => p.Category)
            .HasDatabaseName("idx_products_category");
        builder.HasIndex(p => p.IsAvailable)
            .HasDatabaseName("idx_products_is_available");
        builder.Ignore(p => p.ProductId);
        builder.Ignore(p => p.DomainEvents);
    }
}
