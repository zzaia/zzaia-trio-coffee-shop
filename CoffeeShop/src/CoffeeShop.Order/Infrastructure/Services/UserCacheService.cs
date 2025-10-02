using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Infrastructure.Persistence;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Services;

/// <summary>
/// Implements user cache operations using the Order database.
/// </summary>
public sealed class UserCacheService(
    OrderDbContext dbContext,
    ILogger<UserCacheService> logger) : IUserCacheService
{
    /// <inheritdoc/>
    public async Task CreateOrUpdateUserAsync(
        string userId,
        string email,
        string fullName,
        string role,
        CancellationToken cancellationToken = default)
    {
        try
        {
            User? existingUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

            if (existingUser is not null)
            {
                existingUser.Email = email;
                existingUser.FullName = fullName;
                existingUser.Role = role;
                logger.LogInformation("Updated user {UserId} in local cache", userId);
            }
            else
            {
                User newUser = User.Create(userId, email, fullName, role);
                await dbContext.Users.AddAsync(newUser, cancellationToken);
                logger.LogInformation("Created user {UserId} in local cache", userId);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create or update user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            User? user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

            if (user is not null)
            {
                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Deleted user {UserId} from local cache", userId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete user {UserId}", userId);
            throw;
        }
    }
}
