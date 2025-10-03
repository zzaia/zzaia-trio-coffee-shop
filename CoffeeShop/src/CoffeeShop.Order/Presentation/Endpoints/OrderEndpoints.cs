namespace Zzaia.CoffeeShop.Order.Presentation.Endpoints;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zzaia.CoffeeShop.Order.Application.Commands.CreateOrder;
using Zzaia.CoffeeShop.Order.Application.Commands.UpdateOrderStatus;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Application.Queries.GetAllOrders;
using Zzaia.CoffeeShop.Order.Application.Queries.GetMenu;
using Zzaia.CoffeeShop.Order.Application.Queries.GetOrderById;
using Zzaia.CoffeeShop.Order.Domain.Enums;
using Zzaia.CoffeeShop.Order.Infrastructure.Authentication;

/// <summary>
/// Defines REST API endpoints for order management.
/// </summary>
public static class OrderEndpoints
{
    /// <summary>
    /// Maps all order-related endpoints to the application.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder orderGroup = endpoints.MapGroup("/api/v1/orders")
            .WithTags("Orders")
            .WithOpenApi();

        endpoints.MapGet("/api/v1/menu", GetMenuAsync)
            .WithName("GetMenu")
            .WithSummary("View the complete menu with products and variations")
            .WithDescription("Returns a complete list of products with base prices, all available variations, and price changes for each variation")
            .AllowAnonymous()
            .Produces<MenuDto>(StatusCodes.Status200OK)
            .WithTags("Menu")
            .WithOpenApi();

        orderGroup.MapPost("", CreateOrderAsync)
            .WithName("CreateOrder")
            .WithSummary("Place a new order")
            .WithDescription("Accepts a list of products and variations, processes payment, and creates an order with status 'Waiting'")
            .RequireAuthorization(AuthorizationPolicies.CustomerPolicy)
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .WithOpenApi();

        orderGroup.MapGet("{id:guid}", GetOrderByIdAsync)
            .WithName("GetOrderById")
            .WithSummary("View order details")
            .WithDescription("Returns full information for a specific order including items, variations, pricing, status, and timestamp")
            .RequireAuthorization(AuthorizationPolicies.CustomerPolicy)
            .Produces<OrderDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .WithOpenApi();

        orderGroup.MapGet("", GetAllOrdersAsync)
            .WithName("GetAllOrders")
            .WithSummary("List all orders (Manager only)")
            .WithDescription("Returns a list of all current orders sorted by creation time (oldest first). Requires manager role.")
            .RequireAuthorization(AuthorizationPolicies.ManagerPolicy)
            .Produces<List<OrderDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .WithOpenApi();

        orderGroup.MapPatch("{id:guid}/status", UpdateOrderStatusAsync)
            .WithName("UpdateOrderStatus")
            .WithSummary("Update order status (Manager only)")
            .WithDescription("Updates order status following the sequence: Waiting → Preparation → Ready → Delivered. Requires manager role.")
            .RequireAuthorization(AuthorizationPolicies.ManagerPolicy)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> GetMenuAsync(
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        GetMenuQuery query = new();
        Result<MenuDto> result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Failed to retrieve menu",
                detail: result.Error);
        }
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> CreateOrderAsync(
        [FromBody] CreateOrderRequest request,
        HttpContext httpContext,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        string? userId = httpContext.User.FindFirst("sub")?.Value
            ?? httpContext.User.FindFirst("userId")?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        CreateOrderCommand command = new(
            userId,
            request.Items.Select(item => new OrderItemRequest(
                item.ProductId,
                item.VariationId,
                item.Quantity)).ToList());
        Result<Guid> result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Order creation failed",
                detail: result.Error);
        }
        return Results.Created(
            $"/api/v1/orders/{result.Value}",
            new CreateOrderResponse(result.Value));
    }

    private static async Task<IResult> GetOrderByIdAsync(
        [FromRoute] Guid id,
        HttpContext httpContext,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        string? userId = httpContext.User.FindFirst("sub")?.Value
            ?? httpContext.User.FindFirst("userId")?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        GetOrderByIdQuery query = new(id, userId);
        Result<OrderDto> result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Order not found",
                detail: result.Error);
        }
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetAllOrdersAsync(
        HttpContext httpContext,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        string? userId = httpContext.Request.Headers.TryGetValue("userId", out var sub) ? sub.ToString() :
            httpContext.User.FindFirst("userId")?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        GetAllOrdersQuery query = new(userId, IsManager: true);
        Result<List<OrderDto>> result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Failed to retrieve orders",
                detail: result.Error);
        }
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateOrderStatusAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateOrderStatusRequest request,
        HttpContext httpContext,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        UpdateOrderStatusCommand command = new(id, request.NewStatus);
        Result result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Status update failed",
                detail: result.Error);
        }
        return Results.NoContent();
    }
}

/// <summary>
/// Request model for creating a new order.
/// </summary>
/// <param name="Items">The list of order items.</param>
public record CreateOrderRequest(List<CreateOrderItemRequest> Items);

/// <summary>
/// Request model for creating an order item.
/// </summary>
/// <param name="ProductId">The product identifier.</param>
/// <param name="Quantity">The quantity of the product.</param>
/// <param name="VariationId">The optional product variation identifier.</param>
public record CreateOrderItemRequest(Guid ProductId, int Quantity, Guid? VariationId);

/// <summary>
/// Response model for order creation.
/// </summary>
/// <param name="OrderId">The created order identifier.</param>
public record CreateOrderResponse(Guid OrderId);

/// <summary>
/// Request model for updating order status.
/// </summary>
/// <param name="NewStatus">The new order status.</param>
public record UpdateOrderStatusRequest(OrderStatus NewStatus);
