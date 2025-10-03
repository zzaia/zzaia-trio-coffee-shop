using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
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
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
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
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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
app.UseAuthentication();
app.UseAuthorization();

app.MapOrderEndpoints();
app.MapUserEventSubscriptions();

app.Run();
