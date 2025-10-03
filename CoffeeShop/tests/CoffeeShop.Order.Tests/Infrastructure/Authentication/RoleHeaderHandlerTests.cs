using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Zzaia.CoffeeShop.Order.Infrastructure.Authentication;

namespace Zzaia.CoffeeShop.Order.Tests.Infrastructure.Authentication;

public class RoleHeaderHandlerTests
{
    [Fact]
    public async Task HandleRequirementAsync_WithValidCustomerRole_ShouldSucceed()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        DefaultHttpContext httpContext = new();
        httpContext.Request.Headers["role"] = "Customer";
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        RoleHeaderHandler handler = new(httpContextAccessorMock.Object);
        RoleHeaderRequirement requirement = new("Customer");
        ClaimsPrincipal user = new(new ClaimsIdentity("test"));
        AuthorizationHandlerContext context = new(
            [requirement],
            user,
            null);
        await handler.HandleAsync(context);
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithValidManagerRole_ShouldSucceed()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        DefaultHttpContext httpContext = new();
        httpContext.Request.Headers["role"] = "Manager";
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        RoleHeaderHandler handler = new(httpContextAccessorMock.Object);
        RoleHeaderRequirement requirement = new("Manager");
        ClaimsPrincipal user = new(new ClaimsIdentity("test"));
        AuthorizationHandlerContext context = new(
            [requirement],
            user,
            null);
        await handler.HandleAsync(context);
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithInvalidRole_ShouldFail()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        DefaultHttpContext httpContext = new();
        httpContext.Request.Headers["role"] = "InvalidRole";
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        RoleHeaderHandler handler = new(httpContextAccessorMock.Object);
        RoleHeaderRequirement requirement = new("Customer");
        ClaimsPrincipal user = new(new ClaimsIdentity("test"));
        AuthorizationHandlerContext context = new(
            [requirement],
            user,
            null);
        await handler.HandleAsync(context);
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithMissingRoleHeader_ShouldFail()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        DefaultHttpContext httpContext = new();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        RoleHeaderHandler handler = new(httpContextAccessorMock.Object);
        RoleHeaderRequirement requirement = new("Customer");
        ClaimsPrincipal user = new(new ClaimsIdentity("test"));
        AuthorizationHandlerContext context = new(
            [requirement],
            user,
            null);
        await handler.HandleAsync(context);
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithEmptyRoleHeader_ShouldFail()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        DefaultHttpContext httpContext = new();
        httpContext.Request.Headers["role"] = string.Empty;
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        RoleHeaderHandler handler = new(httpContextAccessorMock.Object);
        RoleHeaderRequirement requirement = new("Customer");
        ClaimsPrincipal user = new(new ClaimsIdentity("test"));
        AuthorizationHandlerContext context = new(
            [requirement],
            user,
            null);
        await handler.HandleAsync(context);
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithNullHttpContext_ShouldFail()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        RoleHeaderHandler handler = new(httpContextAccessorMock.Object);
        RoleHeaderRequirement requirement = new("Customer");
        ClaimsPrincipal user = new(new ClaimsIdentity("test"));
        AuthorizationHandlerContext context = new(
            [requirement],
            user,
            null);
        await handler.HandleAsync(context);
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithCaseInsensitiveRole_ShouldSucceed()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        DefaultHttpContext httpContext = new();
        httpContext.Request.Headers["role"] = "customer";
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        RoleHeaderHandler handler = new(httpContextAccessorMock.Object);
        RoleHeaderRequirement requirement = new("Customer");
        ClaimsPrincipal user = new(new ClaimsIdentity("test"));
        AuthorizationHandlerContext context = new(
            [requirement],
            user,
            null);
        await handler.HandleAsync(context);
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithMultipleAllowedRoles_ShouldSucceed()
    {
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        DefaultHttpContext httpContext = new();
        httpContext.Request.Headers["role"] = "Manager";
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        RoleHeaderHandler handler = new(httpContextAccessorMock.Object);
        RoleHeaderRequirement requirement = new("Customer", "Manager");
        ClaimsPrincipal user = new(new ClaimsIdentity("test"));
        AuthorizationHandlerContext context = new(
            [requirement],
            user,
            null);
        await handler.HandleAsync(context);
        context.HasSucceeded.Should().BeTrue();
    }
}
