namespace Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Represents a cached user from Identity service.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Gets the user email.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets the user full name.
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// Gets the user role.
    /// </summary>
    public required string Role { get; init; }

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }

    private User()
    {
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Creates a new User instance.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="email">The user email.</param>
    /// <param name="fullName">The user full name.</param>
    /// <param name="role">The user role.</param>
    /// <returns>A new User instance.</returns>
    public static User Create(string userId, string email, string fullName, string role)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        }
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));
        }
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ArgumentException("Role cannot be empty.", nameof(role));
        }
        return new User
        {
            UserId = userId,
            Email = email,
            FullName = fullName,
            Role = role
        };
    }
}
