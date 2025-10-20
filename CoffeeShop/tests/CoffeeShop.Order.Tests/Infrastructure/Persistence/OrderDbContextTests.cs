using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Zzaia.CoffeeShop.Order.Infrastructure.Persistence;
using OrderDomain = Zzaia.CoffeeShop.Order.Domain;

namespace Zzaia.CoffeeShop.Order.Tests.Infrastructure.Persistence;

/// <summary>
/// Integration tests for OrderDbContext.
/// </summary>
public sealed class OrderDbContextTests
{
    /// <summary>
    /// Tests that OrderDbContext can be created successfully.
    /// </summary>
    [Fact]
    public void OrderDbContext_ShouldBeCreated_Successfully()
    {
        DbContextOptions<OrderDbContext> options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        Mock<IPublisher> publisherMock = new();
        using OrderDbContext context = new(options, publisherMock.Object);
        context.Should().NotBeNull();
        context.Orders.Should().NotBeNull();
        context.Products.Should().NotBeNull();
        context.ProductVariations.Should().NotBeNull();
        context.Users.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that a product can be added and retrieved from the database.
    /// </summary>
    [Fact]
    public async Task Product_ShouldBeAdded_AndRetrieved()
    {
        DbContextOptions<OrderDbContext> options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        Mock<IPublisher> publisherMock = new();
        using OrderDbContext context = new(options, publisherMock.Object);
        OrderDomain.Entities.Product product = OrderDomain.Entities.Product.Create(
            "Espresso",
            "Rich and bold espresso shot",
            5.00m,
            "Coffee");
        context.Products.Add(product);
        await context.SaveChangesAsync();
        OrderDomain.Entities.Product? retrievedProduct = await context.Products
            .FirstOrDefaultAsync(p => p.Id == product.ProductId);
        retrievedProduct.Should().NotBeNull();
        retrievedProduct!.Name.Should().Be("Espresso");
        retrievedProduct.BasePriceAmount.Should().Be(5.00m);
        retrievedProduct.Category.Should().Be("Coffee");
    }

    /// <summary>
    /// Tests that product variations can be added to a product.
    /// </summary>
    [Fact]
    public async Task ProductVariations_ShouldBeAdded_ToProduct()
    {
        DbContextOptions<OrderDbContext> options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        Mock<IPublisher> publisherMock = new();
        using OrderDbContext context = new(options, publisherMock.Object);
        OrderDomain.Entities.Product product = OrderDomain.Entities.Product.Create(
            "Cappuccino",
            "Espresso with steamed milk and foam",
            7.00m,
            "Coffee");
        OrderDomain.Entities.ProductVariation smallVariation = OrderDomain.Entities.ProductVariation.Create(
            product.ProductId,
            "Small",
            0.00m);
        OrderDomain.Entities.ProductVariation mediumVariation = OrderDomain.Entities.ProductVariation.Create(
            product.ProductId,
            "Medium",
            2.50m);
        context.Products.Add(product);
        context.ProductVariations.Add(smallVariation);
        context.ProductVariations.Add(mediumVariation);
        await context.SaveChangesAsync();
        List<OrderDomain.Entities.ProductVariation> variations = await context.ProductVariations
            .Where(pv => pv.ProductId == product.ProductId)
            .ToListAsync();
        variations.Should().HaveCount(2);
        variations.Should().Contain(v => v.Name == "Small");
        variations.Should().Contain(v => v.Name == "Medium");
    }

    /// <summary>
    /// Tests that an order can be created and saved.
    /// </summary>
    [Fact]
    public async Task Order_ShouldBeCreated_AndSaved()
    {
        DbContextOptions<OrderDbContext> options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        Mock<IPublisher> publisherMock = new();
        using OrderDbContext context = new(options, publisherMock.Object);
        OrderDomain.Entities.Order order = OrderDomain.Entities.Order.Create("user123");
        OrderDomain.ValueObjects.ProductSnapshot snapshot = OrderDomain.ValueObjects.ProductSnapshot.Create(
            Guid.NewGuid(),
            "Latte",
            "Smooth espresso with steamed milk",
            8.00m);
        order.AddItem(snapshot, OrderDomain.ValueObjects.Quantity.Create(2));
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        OrderDomain.Entities.Order? retrievedOrder = await context.Orders
            .FirstOrDefaultAsync(o => o.Id == order.OrderId);
        retrievedOrder.Should().NotBeNull();
        retrievedOrder!.UserId.Should().Be("user123");
        retrievedOrder.TotalAmount.Should().Be(16.00m);
    }

    /// <summary>
    /// Tests that a user can be added to the cache.
    /// </summary>
    [Fact]
    public async Task User_ShouldBeAdded_ToCache()
    {
        DbContextOptions<OrderDbContext> options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        Mock<IPublisher> publisherMock = new();
        using OrderDbContext context = new(options, publisherMock.Object);
        OrderDomain.Entities.User user = OrderDomain.Entities.User.Create(
            "user123",
            "user@example.com",
            "John Doe",
            "Customer");
        context.Users.Add(user);
        await context.SaveChangesAsync();
        OrderDomain.Entities.User? retrievedUser = await context.Users
            .FirstOrDefaultAsync(u => u.UserId == "user123");
        retrievedUser.Should().NotBeNull();
        retrievedUser!.Email.Should().Be("user@example.com");
        retrievedUser.FullName.Should().Be("John Doe");
        retrievedUser.Role.Should().Be("Customer");
    }
}
