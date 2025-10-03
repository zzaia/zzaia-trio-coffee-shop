namespace Zzaia.CoffeeShop.Order.Application.Queries.GetMenu;

using MediatR;
using Microsoft.Extensions.Logging;
using Zzaia.CoffeeShop.Order.Application.Common.Interfaces;
using Zzaia.CoffeeShop.Order.Application.Common.Models;
using Zzaia.CoffeeShop.Order.Domain.Entities;

/// <summary>
/// Handles the GetMenuQuery request.
/// </summary>
public class GetMenuQueryHandler(
    IProductRepository productRepository,
    ILogger<GetMenuQueryHandler> logger) : IRequestHandler<GetMenuQuery, Result<MenuDto>>
{
    /// <summary>
    /// Handles the query execution.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the menu data transfer object.</returns>
    public async Task<Result<MenuDto>> Handle(GetMenuQuery request, CancellationToken cancellationToken)
    {
        try
        {
            List<Product> products = await productRepository.GetAllAsync(cancellationToken);
            List<ProductDto> productDtos = products.Select(product => new ProductDto(
                product.ProductId,
                product.Name,
                product.Description,
                product.BasePriceAmount,
                product.Currency,
                product.Category,
                product.ImageUrl,
                product.IsAvailable,
                product.Variations.Select(variation => new ProductVariationDto(
                    variation.VariationId,
                    variation.Name,
                    variation.PriceAdjustmentAmount,
                    variation.Currency
                )).ToList()
            )).ToList();
            MenuDto menu = new MenuDto(productDtos);
            logger.LogInformation("Retrieved menu with {ProductCount} products", products.Count);
            return Result<MenuDto>.Success(menu);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error retrieving menu");
            return Result<MenuDto>.Failure($"Failed to retrieve menu: {exception.Message}");
        }
    }
}
