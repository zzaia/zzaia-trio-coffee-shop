using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zzaia.CoffeeShop.Order.Infrastructure.Persistence;

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
        return services;
    }
}
