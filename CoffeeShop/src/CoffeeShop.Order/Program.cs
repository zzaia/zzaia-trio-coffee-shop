using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => new { status = "healthy", service = "CoffeeShop.Order" })
    .WithName("GetHealth");

app.MapGet("/orders", () => new[]
{
    new Order(1, "Espresso", 3.50m, DateTimeOffset.UtcNow),
    new Order(2, "Cappuccino", 4.25m, DateTimeOffset.UtcNow.AddMinutes(-5)),
    new Order(3, "Latte", 4.75m, DateTimeOffset.UtcNow.AddMinutes(-10))
})
.WithName("GetOrders");

app.Run();

record Order(int Id, string Name, decimal Price, DateTimeOffset CreatedAt);
