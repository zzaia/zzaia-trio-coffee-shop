using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Domain.Enums;
using Zzaia.CoffeeShop.Order.Domain.ValueObjects;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for Order aggregate.
/// </summary>
internal sealed class OrderConfiguration : IEntityTypeConfiguration<Domain.Entities.Order>
{
    /// <summary>
    /// Configures the Order entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder instance.</param>
    public void Configure(EntityTypeBuilder<Domain.Entities.Order> builder)
    {
        builder.ToTable("orders", "order");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasColumnName("order_id")
            .ValueGeneratedNever();
        builder.Property(o => o.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(o => o.Status)
            .HasColumnName("status")
            .HasConversion(
                status => (int)status,
                value => (OrderStatus)value)
            .IsRequired();
        builder.Property(o => o.PaymentTransactionId)
            .HasColumnName("payment_transaction_id")
            .HasMaxLength(256);
        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(o => o.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("total_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("total_currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        builder.Navigation(o => o.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("items");
        builder.HasMany<OrderItem>(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(o => o.UserId)
            .HasDatabaseName("idx_orders_user_id");
        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("idx_orders_created_at");
        builder.HasIndex(o => o.Status)
            .HasDatabaseName("idx_orders_status");
        builder.Ignore(o => o.OrderId);
        builder.Ignore(o => o.DomainEvents);
    }
}
