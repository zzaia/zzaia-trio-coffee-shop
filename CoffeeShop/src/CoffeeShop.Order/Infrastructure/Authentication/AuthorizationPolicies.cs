using Microsoft.AspNetCore.Authorization;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Authentication;

/// <summary>
/// Defines authorization policies for the Order service.
/// </summary>
public static class AuthorizationPolicies
{
    public const string CustomerPolicy = "CustomerPolicy";
    public const string ManagerPolicy = "ManagerPolicy";

    /// <summary>
    /// Configures authorization policies.
    /// </summary>
    /// <param name="services">The service collection instance.</param>
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IAuthorizationHandler, RoleHeaderHandler>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy(CustomerPolicy, policy =>
            {
                // policy.RequireAuthenticatedUser();
                // policy.RequireClaim("role", "Customer", "customer");
                policy.Requirements.Add(new RoleHeaderRequirement("Customer"));
            });
            options.AddPolicy(ManagerPolicy, policy =>
            {
                // policy.RequireAuthenticatedUser();
                // policy.RequireClaim("role", "Manager", "manager");
                policy.Requirements.Add(new RoleHeaderRequirement("Manager"));
            });
        });
    }
}
