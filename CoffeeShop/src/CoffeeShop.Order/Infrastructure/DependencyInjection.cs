using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Infrastructure.Persistence;
using Zzaia.CoffeeShop.Order.Infrastructure.Persistence.Repositories;
using Zzaia.CoffeeShop.Order.Infrastructure.Services;

namespace Zzaia.CoffeeShop.Order.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection configuration.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The service collection instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection instance for method chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHttpClient<IPaymentService, PaymentService>(client =>
        {
            string baseUrl = configuration["Services:PaymentService:BaseUrl"]
                ?? "http://localhost:5100";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddHttpClient<INotificationService, ExternalNotificationService>(client =>
        {
            string baseUrl = configuration["Services:NotificationService:BaseUrl"]
                ?? "http://localhost:5200";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });
        DaprClientBuilder daprBuilder = new DaprClientBuilder();
        DaprClient daprClient = daprBuilder.Build();
        services.AddSingleton(daprClient);
        services.AddScoped<IIdentityServiceClient, IdentityServiceClient>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<IUserCacheService, UserCacheService>();
        return services;
    }
}
