using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Zzaia.CoffeeShop.Order.Domain.Entities;
using Zzaia.CoffeeShop.Order.Infrastructure.Persistence;
using Zzaia.CoffeeShop.Order.Infrastructure.Services;

namespace Zzaia.CoffeeShop.Order.Tests.Infrastructure.Services;

/// <summary>
/// Unit tests for UserCacheService.
/// </summary>
public sealed class UserCacheServiceTests : IDisposable
{
    private readonly OrderDbContext _dbContext;
    private readonly UserCacheService _service;

    public UserCacheServiceTests()
    {
        DbContextOptions<OrderDbContext> options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Mock<IPublisher> mockPublisher = new();
        _dbContext = new OrderDbContext(options, mockPublisher.Object);
        Mock<ILogger<UserCacheService>> mockLogger = new();
        _service = new UserCacheService(_dbContext, mockLogger.Object);
    }

    [Fact]
    public async Task CreateOrUpdateUserAsync_ShouldCreateNewUser()
    {
        // Arrange
        string userId = "user-123";
        string email = "test@example.com";
        string fullName = "Test User";
        string role = "Customer";

        // Act
        await _service.CreateOrUpdateUserAsync(userId, email, fullName, role);

        // Assert
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        user.Should().NotBeNull();
        user!.Email.Should().Be(email);
        user.FullName.Should().Be(fullName);
        user.Role.Should().Be(role);
    }

    [Fact]
    public async Task CreateOrUpdateUserAsync_ShouldUpdateExistingUser()
    {
        // Arrange
        string userId = "user-123";
        User existingUser = User.Create(userId, "old@example.com", "Old Name", "Customer");
        await _dbContext.Users.AddAsync(existingUser);
        await _dbContext.SaveChangesAsync();

        string newEmail = "new@example.com";
        string newFullName = "New Name";
        string newRole = "Manager";

        // Act
        await _service.CreateOrUpdateUserAsync(userId, newEmail, newFullName, newRole);

        // Assert
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        user.Should().NotBeNull();
        user!.Email.Should().Be(newEmail);
        user.FullName.Should().Be(newFullName);
        user.Role.Should().Be(newRole);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldRemoveUser()
    {
        // Arrange
        string userId = "user-123";
        User user = User.Create(userId, "test@example.com", "Test User", "Customer");
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        await _service.DeleteUserAsync(userId);

        // Assert
        User? deletedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldHandleNonExistentUser()
    {
        // Arrange
        string userId = "non-existent-user";

        // Act
        Func<Task> act = async () => await _service.DeleteUserAsync(userId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
