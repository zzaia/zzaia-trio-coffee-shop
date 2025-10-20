using MediatR;
using Microsoft.EntityFrameworkCore;
using Zzaia.CoffeeShop.Order.Domain.Common;
using Zzaia.CoffeeShop.Order.Domain.Entities;

namespace Zzaia.CoffeeShop.Order.Infrastructure.Persistence;

/// <summary>
/// Database context for Order service.
/// </summary>
public sealed class OrderDbContext(
    DbContextOptions<OrderDbContext> options,
    IPublisher publisher) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Orders DbSet.
    /// </summary>
    public DbSet<Domain.Entities.Order> Orders { get; set; }

    /// <summary>
    /// Gets or sets the OrderItems DbSet.
    /// </summary>
    public DbSet<OrderItem> OrderItems { get; set; }

    /// <summary>
    /// Gets or sets the Products DbSet.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Gets or sets the ProductVariations DbSet.
    /// </summary>
    public DbSet<ProductVariation> ProductVariations { get; set; }

    /// <summary>
    /// Gets or sets the Users DbSet.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Configures entity mappings and relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("order");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }

    /// <summary>
    /// Saves changes to the database with domain event dispatching.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //TODO: No need at the moment, but if you want to implement auditing or domain event dispatching, you can uncomment these lines.
        // UpdateTimestamps();
        // await DispatchDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> entries = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry in entries)
        {
            if (entry.Entity is Domain.Entities.Order order)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = now;
                }
                entry.Property("UpdatedAt").CurrentValue = now;
            }
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        List<Entity> entities = ChangeTracker.Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();
        List<IDomainEvent> domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();
        entities.ForEach(e => e.ClearDomainEvents());
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
