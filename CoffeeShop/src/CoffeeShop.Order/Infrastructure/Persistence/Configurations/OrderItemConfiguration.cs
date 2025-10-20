using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for OrderItem.
/// </summary>
internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    /// <summary>
    /// Configures the OrderItem entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder instance.</param>
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items", "order");
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id)
            .HasColumnName("order_item_id")
            .ValueGeneratedNever();
        builder.Property(oi => oi.OrderId)
            .HasColumnName("order_id")
            .IsRequired();
        builder.OwnsOne(oi => oi.ProductSnapshot, snapshot =>
        {
            snapshot.Property(s => s.ProductId)
                .HasColumnName("product_id")
                .IsRequired();
            snapshot.Property(s => s.Name)
                .HasColumnName("product_name")
                .HasMaxLength(200)
                .IsRequired();
            snapshot.Property(s => s.Description)
                .HasColumnName("product_description")
                .HasMaxLength(1000)
                .IsRequired();
            snapshot.Property(s => s.VariationName)
                .HasColumnName("variation_name")
                .HasMaxLength(200);
            snapshot.Property(s => s.UnitPriceAmount)
                .HasColumnName("unit_price_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            snapshot.Property(s => s.Currency)
                .HasColumnName("unit_price_currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        builder.Property(oi => oi.Quantity)
            .HasColumnName("quantity")
            .HasConversion(
                quantity => quantity.Value,
                value => Quantity.Create(value))
            .IsRequired();
        builder.Property(oi => oi.SubtotalAmount)
            .HasColumnName("subtotal_amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(oi => oi.Currency)
            .HasColumnName("subtotal_currency")
            .HasMaxLength(3)
            .IsRequired();
        builder.Ignore(oi => oi.OrderItemId);
        builder.Ignore(oi => oi.DomainEvents);
    }
}
