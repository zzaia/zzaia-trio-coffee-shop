using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Zzaia.CoffeeShop.Order.Infrastructure.Authentication;

namespace Zzaia.CoffeeShop.Order.Tests.Infrastructure.Authentication;

public class AuthorizationPoliciesTests
{
    /// <summary>
    /// Commented because we are not using the authenticated user requirement for now.
    /// </summary>
    // [Fact]
    // public void AddAuthorizationPolicies_ShouldRegisterCustomerPolicy()
    // {
    //     ServiceCollection services = new();
    //     services.AddAuthorizationPolicies();
    //     ServiceProvider provider = services.BuildServiceProvider();
    //     IAuthorizationPolicyProvider policyProvider = provider
    //         .GetRequiredService<IAuthorizationPolicyProvider>();
    //     AuthorizationPolicy? policy = policyProvider
    //         .GetPolicyAsync(AuthorizationPolicies.CustomerPolicy)
    //         .GetAwaiter()
    //         .GetResult();
    //     policy.Should().NotBeNull();
    //     policy!.Requirements.Should().HaveCount(2);
    // }

    // [Fact]
    // public void AddAuthorizationPolicies_ShouldRegisterManagerPolicy()
    // {
    //     ServiceCollection services = new();
    //     services.AddAuthorizationPolicies();
    //     ServiceProvider provider = services.BuildServiceProvider();
    //     IAuthorizationPolicyProvider policyProvider = provider
    //         .GetRequiredService<IAuthorizationPolicyProvider>();
    //     AuthorizationPolicy? policy = policyProvider
    //         .GetPolicyAsync(AuthorizationPolicies.ManagerPolicy)
    //         .GetAwaiter()
    //         .GetResult();
    //     policy.Should().NotBeNull();
    //     policy!.Requirements.Should().HaveCount(2);
    // }

    [Fact]
    public void CustomerPolicy_ShouldContainRoleHeaderRequirement()
    {
        ServiceCollection services = new();
        services.AddAuthorizationPolicies();
        ServiceProvider provider = services.BuildServiceProvider();
        IAuthorizationPolicyProvider policyProvider = provider
            .GetRequiredService<IAuthorizationPolicyProvider>();
        AuthorizationPolicy? policy = policyProvider
            .GetPolicyAsync(AuthorizationPolicies.CustomerPolicy)
            .GetAwaiter()
            .GetResult();
        policy.Should().NotBeNull();
        RoleHeaderRequirement? roleRequirement = policy!.Requirements
            .OfType<RoleHeaderRequirement>()
            .FirstOrDefault();
        roleRequirement.Should().NotBeNull();
        roleRequirement!.AllowedRoles.Should().Contain("Customer");
    }

    [Fact]
    public void ManagerPolicy_ShouldContainRoleHeaderRequirement()
    {
        ServiceCollection services = new();
        services.AddAuthorizationPolicies();
        ServiceProvider provider = services.BuildServiceProvider();
        IAuthorizationPolicyProvider policyProvider = provider
            .GetRequiredService<IAuthorizationPolicyProvider>();
        AuthorizationPolicy? policy = policyProvider
            .GetPolicyAsync(AuthorizationPolicies.ManagerPolicy)
            .GetAwaiter()
            .GetResult();
        policy.Should().NotBeNull();
        RoleHeaderRequirement? roleRequirement = policy!.Requirements
            .OfType<RoleHeaderRequirement>()
            .FirstOrDefault();
        roleRequirement.Should().NotBeNull();
        roleRequirement!.AllowedRoles.Should().Contain("Manager");
    }

    [Fact]
    public void AddAuthorizationPolicies_ShouldRegisterRoleHeaderHandler()
    {
        ServiceCollection services = new();
        services.AddAuthorizationPolicies();
        ServiceProvider provider = services.BuildServiceProvider();
        IEnumerable<IAuthorizationHandler> handlers = provider
            .GetServices<IAuthorizationHandler>();
        handlers.Should().Contain(h => h is RoleHeaderHandler);
    }
}
