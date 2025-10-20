using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();

app.MapGet("/health", () => new { status = "healthy", service = "CoffeeShop.BFF" })
    .WithName("GetHealth");

app.MapGet("/api/menu", () => new[]
{
    new MenuItem(1, "Espresso", "Strong black coffee", 3.50m, "Coffee"),
    new MenuItem(2, "Cappuccino", "Espresso with steamed milk foam", 4.25m, "Coffee"),
    new MenuItem(3, "Latte", "Espresso with steamed milk", 4.75m, "Coffee"),
    new MenuItem(4, "Americano", "Espresso with hot water", 3.75m, "Coffee"),
    new MenuItem(5, "Croissant", "Buttery flaky pastry", 2.95m, "Pastry")
})
.WithName("GetMenu");

app.Run();

record MenuItem(int Id, string Name, string Description, decimal Price, string Category);
