using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // Seed Users
            migrationBuilder.InsertData(
                schema: "order",
                table: "users",
                columns: new[] { "user_id", "email", "full_name", "role", "created_at", "updated_at" },
                values: new object[,]
                {
                    { "customer-001", "john.smith@coffeeshop.com", "John Smith", "Customer", now, now },
                    { "customer-002", "emma.johnson@coffeeshop.com", "Emma Johnson", "Customer", now, now },
                    { "customer-003", "michael.brown@coffeeshop.com", "Michael Brown", "Customer", now, now },
                    { "manager-001", "sarah.davis@coffeeshop.com", "Sarah Davis", "Manager", now, now },
                    { "manager-002", "robert.wilson@coffeeshop.com", "Robert Wilson", "Manager", now, now }
                });

            // Seed Products - Exact catalog from order-architecture.md
            migrationBuilder.InsertData(
                schema: "order",
                table: "products",
                columns: new[] { "product_id", "name", "description", "base_price_amount", "base_price_currency", "category", "image_url", "is_available" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Latte", "Smooth latte with steamed milk", 4.00m, "USD", "Coffee", null, true },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Espresso", "Rich and bold espresso shot", 2.50m, "USD", "Coffee", null, true },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Macchiato", "Espresso with a dollop of foam", 4.00m, "USD", "Coffee", null, true },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Iced Coffee", "Refreshing cold brewed coffee", 3.50m, "USD", "Coffee", null, true },
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Donuts", "Fresh baked donuts", 2.00m, "USD", "Food", null, true }
                });

            // Seed Product Variations - Exact variations from order-architecture.md
            migrationBuilder.InsertData(
                schema: "order",
                table: "product_variations",
                columns: new[] { "variation_id", "product_id", "name", "price_adjustment_amount", "price_adjustment_currency" },
                values: new object[,]
                {
                    // Latte variations (3)
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), "Pumpkin Spice", 0.50m, "USD" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001"), "Vanilla", 0.30m, "USD" },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001"), "Hazelnut", 0.40m, "USD" },

                    // Espresso variations (2)
                    { new Guid("30000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002"), "Single Shot", 0.00m, "USD" },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000002"), "Double Shot", 1.00m, "USD" },

                    // Macchiato variations (2)
                    { new Guid("30000000-0000-0000-0000-000000000006"), new Guid("10000000-0000-0000-0000-000000000003"), "Caramel", 0.50m, "USD" },
                    { new Guid("30000000-0000-0000-0000-000000000007"), new Guid("10000000-0000-0000-0000-000000000003"), "Vanilla", 0.30m, "USD" },

                    // Iced Coffee variations (3)
                    { new Guid("30000000-0000-0000-0000-000000000008"), new Guid("10000000-0000-0000-0000-000000000004"), "Regular", 0.00m, "USD" },
                    { new Guid("30000000-0000-0000-0000-000000000009"), new Guid("10000000-0000-0000-0000-000000000004"), "Sweetened", 0.30m, "USD" },
                    { new Guid("30000000-0000-0000-0000-00000000000A"), new Guid("10000000-0000-0000-0000-000000000004"), "Extra Ice", 0.20m, "USD" },

                    // Donuts variations (3)
                    { new Guid("30000000-0000-0000-0000-00000000000B"), new Guid("20000000-0000-0000-0000-000000000001"), "Glazed", 0.00m, "USD" },
                    { new Guid("30000000-0000-0000-0000-00000000000C"), new Guid("20000000-0000-0000-0000-000000000001"), "Jelly", 0.30m, "USD" },
                    { new Guid("30000000-0000-0000-0000-00000000000D"), new Guid("20000000-0000-0000-0000-000000000001"), "Boston Cream", 0.50m, "USD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete product variations
            migrationBuilder.DeleteData(
                schema: "order",
                table: "product_variations",
                keyColumn: "variation_id",
                keyValues: new object[]
                {
                    new Guid("30000000-0000-0000-0000-000000000001"),
                    new Guid("30000000-0000-0000-0000-000000000002"),
                    new Guid("30000000-0000-0000-0000-000000000003"),
                    new Guid("30000000-0000-0000-0000-000000000004"),
                    new Guid("30000000-0000-0000-0000-000000000005"),
                    new Guid("30000000-0000-0000-0000-000000000006"),
                    new Guid("30000000-0000-0000-0000-000000000007"),
                    new Guid("30000000-0000-0000-0000-000000000008"),
                    new Guid("30000000-0000-0000-0000-000000000009"),
                    new Guid("30000000-0000-0000-0000-00000000000A"),
                    new Guid("30000000-0000-0000-0000-00000000000B"),
                    new Guid("30000000-0000-0000-0000-00000000000C"),
                    new Guid("30000000-0000-0000-0000-00000000000D")
                });

            // Delete products
            migrationBuilder.DeleteData(
                schema: "order",
                table: "products",
                keyColumn: "product_id",
                keyValues: new object[]
                {
                    new Guid("10000000-0000-0000-0000-000000000001"),
                    new Guid("10000000-0000-0000-0000-000000000002"),
                    new Guid("10000000-0000-0000-0000-000000000003"),
                    new Guid("10000000-0000-0000-0000-000000000004"),
                    new Guid("20000000-0000-0000-0000-000000000001")
                });

            // Delete users
            migrationBuilder.DeleteData(
                schema: "order",
                table: "users",
                keyColumn: "user_id",
                keyValues: new object[]
                {
                    "customer-001",
                    "customer-002",
                    "customer-003",
                    "manager-001",
                    "manager-002"
                });
        }
    }
}
