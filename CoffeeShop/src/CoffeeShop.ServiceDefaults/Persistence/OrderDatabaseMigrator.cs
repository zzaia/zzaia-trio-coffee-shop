using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zzaia.CoffeeShop.ServiceDefaults.Persistence;

/// <summary>
/// Background service that applies database migrations for Order context on startup.
/// </summary>
/// <typeparam name="TContext">The DbContext type to migrate.</typeparam>
internal sealed class DatabaseMigrator<TContext>(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime lifetime,
    ILogger<DatabaseMigrator<TContext>> logger) : BackgroundService
    where TContext : DbContext
{
    /// <summary>
    /// Executes the migration task when the service starts.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TaskCompletionSource tcs = new();
        lifetime.ApplicationStarted.Register(() => tcs.SetResult());
        await tcs.Task;
        using IServiceScope scope = serviceProvider.CreateScope();
        TContext context = scope.ServiceProvider.GetRequiredService<TContext>();
        try
        {
            logger.LogInformation("Applying database migrations for {ContextType}", typeof(TContext).Name);
            await context.Database.MigrateAsync(stoppingToken);
            logger.LogInformation("Database migrations applied successfully for {ContextType}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations for {ContextType}", typeof(TContext).Name);
            throw;
        }
    }
}
