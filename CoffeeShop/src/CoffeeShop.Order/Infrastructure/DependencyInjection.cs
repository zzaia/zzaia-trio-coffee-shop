using Dapr.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Infrastructure.Authentication;
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
        services.AddHttpClient<ITokenService, TokenService>();
        services.AddAuthenticationServices(configuration);
        return services;
    }

    /// <summary>
    /// Adds authentication and authorization services.
    /// </summary>
    /// <param name="services">The service collection instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection instance for method chaining.</returns>
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        JwtBearerConfiguration jwtConfig = configuration
            .GetSection(JwtBearerConfiguration.SectionName)
            .Get<JwtBearerConfiguration>() ?? new JwtBearerConfiguration();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = jwtConfig.Authority;
                options.Audience = jwtConfig.Audience;
                options.RequireHttpsMetadata = jwtConfig.RequireHttpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtConfig.ValidateIssuer,
                    ValidateAudience = jwtConfig.ValidateAudience,
                    ValidateLifetime = jwtConfig.ValidateLifetime,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        ILogger<Program> logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();
                        logger.LogError(
                            context.Exception,
                            "Authentication failed: {Message}",
                            context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        ILogger<Program> logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();
                        logger.LogInformation(
                            "Token validated for user: {User}",
                            context.Principal?.Identity?.Name ?? "Unknown");
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorizationPolicies();
        return services;
    }
}
