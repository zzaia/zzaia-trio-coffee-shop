using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete existing seed data
            migrationBuilder.Sql(@"
                DELETE FROM ""order"".""product_variations"";
                DELETE FROM ""order"".""products"";
            ");

            // Product IDs
            Guid latteId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            Guid espressoId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            Guid macchiatoId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            Guid icedCoffeeId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            Guid donutsId = Guid.Parse("55555555-5555-5555-5555-555555555555");

            // Insert Products
            migrationBuilder.InsertData(
                schema: "order",
                table: "products",
                columns: new[] { "product_id", "name", "description", "base_price_amount", "base_price_currency", "category", "image_url", "is_available" },
                values: new object[,]
                {
                    { latteId, "Latte", "Smooth espresso with steamed milk", 4.00m, "USD", "Coffee", null, true },
                    { espressoId, "Espresso", "Rich and bold espresso shot", 2.50m, "USD", "Coffee", null, true },
                    { macchiatoId, "Macchiato", "Espresso with a touch of steamed milk", 4.00m, "USD", "Coffee", null, true },
                    { icedCoffeeId, "Iced Coffee", "Refreshing cold-brewed coffee", 3.50m, "USD", "Coffee", null, true },
                    { donutsId, "Donuts", "Freshly baked donuts", 2.00m, "USD", "Pastry", null, true }
                });

            // Insert Product Variations
            migrationBuilder.InsertData(
                schema: "order",
                table: "product_variations",
                columns: new[] { "variation_id", "product_id", "name", "price_adjustment_amount", "price_adjustment_currency" },
                values: new object[,]
                {
                    // Latte Variations
                    { Guid.Parse("a1111111-1111-1111-1111-111111111111"), latteId, "Pumpkin Spice", 0.50m, "USD" },
                    { Guid.Parse("a1111111-1111-1111-1111-111111111112"), latteId, "Vanilla", 0.30m, "USD" },
                    { Guid.Parse("a1111111-1111-1111-1111-111111111113"), latteId, "Hazelnut", 0.40m, "USD" },

                    // Espresso Variations
                    { Guid.Parse("a2222222-2222-2222-2222-222222222221"), espressoId, "Single Shot", 0.00m, "USD" },
                    { Guid.Parse("a2222222-2222-2222-2222-222222222222"), espressoId, "Double Shot", 1.00m, "USD" },

                    // Macchiato Variations
                    { Guid.Parse("a3333333-3333-3333-3333-333333333331"), macchiatoId, "Caramel", 0.50m, "USD" },
                    { Guid.Parse("a3333333-3333-3333-3333-333333333332"), macchiatoId, "Vanilla", 0.30m, "USD" },

                    // Iced Coffee Variations
                    { Guid.Parse("a4444444-4444-4444-4444-444444444441"), icedCoffeeId, "Regular", 0.00m, "USD" },
                    { Guid.Parse("a4444444-4444-4444-4444-444444444442"), icedCoffeeId, "Sweetened", 0.30m, "USD" },
                    { Guid.Parse("a4444444-4444-4444-4444-444444444443"), icedCoffeeId, "Extra Ice", 0.20m, "USD" },

                    // Donuts Variations
                    { Guid.Parse("a5555555-5555-5555-5555-555555555551"), donutsId, "Glazed", 0.00m, "USD" },
                    { Guid.Parse("a5555555-5555-5555-5555-555555555552"), donutsId, "Jelly", 0.30m, "USD" },
                    { Guid.Parse("a5555555-5555-5555-5555-555555555553"), donutsId, "Boston Cream", 0.50m, "USD" }
                });
        }
    }
}
