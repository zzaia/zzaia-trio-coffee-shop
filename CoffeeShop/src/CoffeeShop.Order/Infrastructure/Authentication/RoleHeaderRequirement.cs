using Microsoft.AspNetCore.Authorization;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Authentication;

/// <summary>
/// Authorization requirement that validates role from HTTP header.
/// </summary>
public class RoleHeaderRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleHeaderRequirement"/> class.
    /// </summary>
    /// <param name="allowedRoles">The allowed role values.</param>
    public RoleHeaderRequirement(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }

    /// <summary>
    /// Gets the allowed role values.
    /// </summary>
    public string[] AllowedRoles { get; }
}
