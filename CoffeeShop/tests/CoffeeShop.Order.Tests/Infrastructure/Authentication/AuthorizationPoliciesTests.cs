using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Zzaia.CoffeeShop.Order.Infrastructure.Authentication;

namespace Zzaia.CoffeeShop.Order.Tests.Infrastructure.Authentication;

public class AuthorizationPoliciesTests
{
    [Fact]
    public void AddAuthorizationPolicies_ShouldRegisterCustomerPolicy()
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
        policy!.Requirements.Should().HaveCount(2);
    }

    [Fact]
    public void AddAuthorizationPolicies_ShouldRegisterManagerPolicy()
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
        policy!.Requirements.Should().HaveCount(2);
    }

    [Fact]
    public void CustomerPolicy_ShouldRequireAuthenticatedUser()
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
        policy!.AuthenticationSchemes.Should().BeEmpty();
    }

    [Fact]
    public void ManagerPolicy_ShouldRequireAuthenticatedUser()
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
        policy!.AuthenticationSchemes.Should().BeEmpty();
    }
}
