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
            Guid espressoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            Guid cappuccinoId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            Guid latteId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            Guid mochaId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            Guid americanoId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            migrationBuilder.InsertData(
                schema: "order",
                table: "products",
                columns: new[] { "product_id", "name", "description", "base_price_amount", "base_price_currency", "category", "image_url", "is_available" },
                values: new object[,]
                {
                    { espressoId, "Espresso", "Rich and bold espresso shot", 5.00m, "BRL", "Coffee", null, true },
                    { cappuccinoId, "Cappuccino", "Espresso with steamed milk and foam", 7.00m, "BRL", "Coffee", null, true },
                    { latteId, "Latte", "Smooth espresso with steamed milk", 8.00m, "BRL", "Coffee", null, true },
                    { mochaId, "Mocha", "Chocolate espresso with steamed milk", 9.00m, "BRL", "Coffee", null, true },
                    { americanoId, "Americano", "Espresso diluted with hot water", 6.00m, "BRL", "Coffee", null, true }
                });
            migrationBuilder.InsertData(
                schema: "order",
                table: "product_variations",
                columns: new[] { "variation_id", "product_id", "name", "price_adjustment_amount", "price_adjustment_currency" },
                values: new object[,]
                {
                    { Guid.Parse("a1111111-1111-1111-1111-111111111111"), espressoId, "Small", 0.00m, "BRL" },
                    { Guid.Parse("a1111111-1111-1111-1111-111111111112"), espressoId, "Medium", 2.00m, "BRL" },
                    { Guid.Parse("a1111111-1111-1111-1111-111111111113"), espressoId, "Large", 4.00m, "BRL" },
                    { Guid.Parse("a2222222-2222-2222-2222-222222222221"), cappuccinoId, "Small", 0.00m, "BRL" },
                    { Guid.Parse("a2222222-2222-2222-2222-222222222222"), cappuccinoId, "Medium", 2.50m, "BRL" },
                    { Guid.Parse("a2222222-2222-2222-2222-222222222223"), cappuccinoId, "Large", 5.00m, "BRL" },
                    { Guid.Parse("a3333333-3333-3333-3333-333333333331"), latteId, "Small", 0.00m, "BRL" },
                    { Guid.Parse("a3333333-3333-3333-3333-333333333332"), latteId, "Medium", 2.50m, "BRL" },
                    { Guid.Parse("a3333333-3333-3333-3333-333333333333"), latteId, "Large", 5.00m, "BRL" },
                    { Guid.Parse("a4444444-4444-4444-4444-444444444441"), mochaId, "Small", 0.00m, "BRL" },
                    { Guid.Parse("a4444444-4444-4444-4444-444444444442"), mochaId, "Medium", 2.50m, "BRL" },
                    { Guid.Parse("a4444444-4444-4444-4444-444444444443"), mochaId, "Large", 5.00m, "BRL" },
                    { Guid.Parse("a5555555-5555-5555-5555-555555555551"), americanoId, "Small", 0.00m, "BRL" },
                    { Guid.Parse("a5555555-5555-5555-5555-555555555552"), americanoId, "Medium", 2.00m, "BRL" },
                    { Guid.Parse("a5555555-5555-5555-5555-555555555553"), americanoId, "Large", 4.00m, "BRL" },
                    { Guid.Parse("b1111111-1111-1111-1111-111111111111"), espressoId, "Extra Shot", 3.00m, "BRL" },
                    { Guid.Parse("b2222222-2222-2222-2222-222222222222"), cappuccinoId, "Extra Shot", 3.00m, "BRL" },
                    { Guid.Parse("b3333333-3333-3333-3333-333333333333"), latteId, "Extra Shot", 3.00m, "BRL" },
                    { Guid.Parse("b4444444-4444-4444-4444-444444444444"), mochaId, "Extra Shot", 3.00m, "BRL" },
                    { Guid.Parse("b5555555-5555-5555-5555-555555555555"), americanoId, "Extra Shot", 3.00m, "BRL" },
                    { Guid.Parse("c1111111-1111-1111-1111-111111111111"), espressoId, "Soy Milk", 2.00m, "BRL" },
                    { Guid.Parse("c2222222-2222-2222-2222-222222222222"), cappuccinoId, "Soy Milk", 2.00m, "BRL" },
                    { Guid.Parse("c3333333-3333-3333-3333-333333333333"), latteId, "Soy Milk", 2.00m, "BRL" },
                    { Guid.Parse("c4444444-4444-4444-4444-444444444444"), mochaId, "Soy Milk", 2.00m, "BRL" },
                    { Guid.Parse("d1111111-1111-1111-1111-111111111111"), espressoId, "Almond Milk", 2.50m, "BRL" },
                    { Guid.Parse("d2222222-2222-2222-2222-222222222222"), cappuccinoId, "Almond Milk", 2.50m, "BRL" },
                    { Guid.Parse("d3333333-3333-3333-3333-333333333333"), latteId, "Almond Milk", 2.50m, "BRL" },
                    { Guid.Parse("d4444444-4444-4444-4444-444444444444"), mochaId, "Almond Milk", 2.50m, "BRL" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "order",
                table: "product_variations",
                keyColumn: "variation_id",
                keyValues: new object[]
                {
                    Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                    Guid.Parse("a1111111-1111-1111-1111-111111111112"),
                    Guid.Parse("a1111111-1111-1111-1111-111111111113"),
                    Guid.Parse("a2222222-2222-2222-2222-222222222221"),
                    Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                    Guid.Parse("a2222222-2222-2222-2222-222222222223"),
                    Guid.Parse("a3333333-3333-3333-3333-333333333331"),
                    Guid.Parse("a3333333-3333-3333-3333-333333333332"),
                    Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                    Guid.Parse("a4444444-4444-4444-4444-444444444441"),
                    Guid.Parse("a4444444-4444-4444-4444-444444444442"),
                    Guid.Parse("a4444444-4444-4444-4444-444444444443"),
                    Guid.Parse("a5555555-5555-5555-5555-555555555551"),
                    Guid.Parse("a5555555-5555-5555-5555-555555555552"),
                    Guid.Parse("a5555555-5555-5555-5555-555555555553"),
                    Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                    Guid.Parse("b2222222-2222-2222-2222-222222222222"),
                    Guid.Parse("b3333333-3333-3333-3333-333333333333"),
                    Guid.Parse("b4444444-4444-4444-4444-444444444444"),
                    Guid.Parse("b5555555-5555-5555-5555-555555555555"),
                    Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                    Guid.Parse("c2222222-2222-2222-2222-222222222222"),
                    Guid.Parse("c3333333-3333-3333-3333-333333333333"),
                    Guid.Parse("c4444444-4444-4444-4444-444444444444"),
                    Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                    Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                    Guid.Parse("d3333333-3333-3333-3333-333333333333"),
                    Guid.Parse("d4444444-4444-4444-4444-444444444444")
                });
            migrationBuilder.DeleteData(
                schema: "order",
                table: "products",
                keyColumn: "product_id",
                keyValues: new object[]
                {
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Guid.Parse("55555555-5555-5555-5555-555555555555")
                });
        }
    }
}
