namespace Zzaia.CoffeeShop.Order.Application.Queries.GetMenu;

using MediatR;
using Zzaia.CoffeeShop.Order.Application.Common.Models;

/// <summary>
/// Represents a query to retrieve the menu.
/// </summary>
public record GetMenuQuery() : IRequest<Result<MenuDto>>;
