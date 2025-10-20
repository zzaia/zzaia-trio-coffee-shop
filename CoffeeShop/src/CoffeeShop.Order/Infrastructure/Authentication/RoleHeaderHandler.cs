using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Authentication;

/// <summary>
/// Authorization handler that validates role from HTTP header.
/// </summary>
public class RoleHeaderHandler(IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<RoleHeaderRequirement>
{
    private const string RoleHeaderName = "role";

    /// <summary>
    /// Handles the authorization requirement by validating the role header.
    /// </summary>
    /// <param name="context">The authorization handler context.</param>
    /// <param name="requirement">The role header requirement.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleHeaderRequirement requirement)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return Task.CompletedTask;
        }
        if (!httpContext.Request.Headers.TryGetValue(RoleHeaderName, out Microsoft.Extensions.Primitives.StringValues roleHeaderValue))
        {
            return Task.CompletedTask;
        }
        string roleValue = roleHeaderValue.ToString();
        if (string.IsNullOrWhiteSpace(roleValue))
        {
            return Task.CompletedTask;
        }
        if (requirement.AllowedRoles.Contains(roleValue, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
