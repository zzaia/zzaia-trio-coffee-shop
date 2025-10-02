using Dapr;
using Microsoft.AspNetCore.Mvc;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;

namespace Zzaia.CoffeeShop.Order.Presentation.Endpoints;

/// <summary>
/// Defines Dapr pub/sub subscriptions for user events.
/// </summary>
public static class UserEventSubscriptions
{
    /// <summary>
    /// Maps user event subscription endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapUserEventSubscriptions(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/events/user-created", [Topic("order-pubsub", "user.created")] async (
            [FromBody] UserCreatedEvent userEvent,
            [FromServices] IUserCacheService userCacheService,
            CancellationToken cancellationToken) =>
        {
            await userCacheService.CreateOrUpdateUserAsync(
                userEvent.UserId,
                userEvent.Email,
                userEvent.FullName,
                userEvent.Role,
                cancellationToken);
            return Results.Ok();
        }).ExcludeFromDescription();

        endpoints.MapPost("/events/user-updated", [Topic("order-pubsub", "user.updated")] async (
            [FromBody] UserUpdatedEvent userEvent,
            [FromServices] IUserCacheService userCacheService,
            CancellationToken cancellationToken) =>
        {
            await userCacheService.CreateOrUpdateUserAsync(
                userEvent.UserId,
                userEvent.Email,
                userEvent.FullName,
                userEvent.Role,
                cancellationToken);
            return Results.Ok();
        }).ExcludeFromDescription();

        endpoints.MapPost("/events/user-deleted", [Topic("order-pubsub", "user.deleted")] async (
            [FromBody] UserDeletedEvent userEvent,
            [FromServices] IUserCacheService userCacheService,
            CancellationToken cancellationToken) =>
        {
            await userCacheService.DeleteUserAsync(userEvent.UserId, cancellationToken);
            return Results.Ok();
        }).ExcludeFromDescription();
    }
}

/// <summary>
/// Represents a user created event.
/// </summary>
/// <param name="UserId">The user identifier.</param>
/// <param name="Email">The user email.</param>
/// <param name="FullName">The user full name.</param>
/// <param name="Role">The user role.</param>
public record UserCreatedEvent(string UserId, string Email, string FullName, string Role);

/// <summary>
/// Represents a user updated event.
/// </summary>
/// <param name="UserId">The user identifier.</param>
/// <param name="Email">The user email.</param>
/// <param name="FullName">The user full name.</param>
/// <param name="Role">The user role.</param>
public record UserUpdatedEvent(string UserId, string Email, string FullName, string Role);

/// <summary>
/// Represents a user deleted event.
/// </summary>
/// <param name="UserId">The user identifier.</param>
public record UserDeletedEvent(string UserId);
