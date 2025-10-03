using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zzaia.CoffeeShop.Order.Domain.Entities;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for ProductVariation.
/// </summary>
internal sealed class ProductVariationConfiguration : IEntityTypeConfiguration<ProductVariation>
{
    /// <summary>
    /// Configures the ProductVariation entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder instance.</param>
    public void Configure(EntityTypeBuilder<ProductVariation> builder)
    {
        builder.ToTable("product_variations", "order");
        builder.HasKey(pv => pv.Id);
        builder.Property(pv => pv.Id)
            .HasColumnName("variation_id")
            .ValueGeneratedNever();
        builder.Property(pv => pv.ProductId)
            .HasColumnName("product_id")
            .IsRequired();
        builder.Property(pv => pv.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(pv => pv.PriceAdjustmentAmount)
            .HasColumnName("price_adjustment_amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(pv => pv.Currency)
            .HasColumnName("price_adjustment_currency")
            .HasMaxLength(3)
            .IsRequired();
        builder.Ignore(pv => pv.VariationId);
        builder.Ignore(pv => pv.DomainEvents);
    }
}
