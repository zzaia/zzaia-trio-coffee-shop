using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Zzaia.CoffeeShop.Order.Application;
using Zzaia.CoffeeShop.Order.Infrastructure;
using Zzaia.CoffeeShop.Order.Infrastructure.Middleware;
using Zzaia.CoffeeShop.Order.Infrastructure.Persistence;
using Zzaia.CoffeeShop.Order.Presentation.Endpoints;
using Zzaia.CoffeeShop.ServiceDefaults.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddPostgreSqlPersistence<OrderDbContext>("db-order");
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "CoffeeShop Order API",
        Description = "RESTful API for managing coffee shop orders",
        Contact = new OpenApiContact
        {
            Name = "CoffeeShop Team"
        }
    });
    options.AddSecurityDefinition("Role", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "role",
        Type = SecuritySchemeType.ApiKey,
        Description = "User role (customer or manager)"
    });
});

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CoffeeShop Order API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();

app.MapOrderEndpoints();
app.MapUserEventSubscriptions();

app.Run();
