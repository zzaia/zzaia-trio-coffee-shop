using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Zzaia.CoffeeShop.ServiceDefaults.Persistence;

public static class PersistenceExtensions
{
    /// <summary>
    /// Adds a SQL Server DbContext with automatic migrations in Development environment.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type to register</typeparam>
    /// <param name="builder">The host application builder instance</param>
    /// <param name="connectionName">The name of the connection string in configuration</param>
    /// <returns>The builder instance for method chaining</returns>
    public static IHostApplicationBuilder AddPersistence<TContext>(
        this IHostApplicationBuilder builder,
        string connectionName)
        where TContext : DbContext
    {
        string? connectionString = builder.Configuration.GetConnectionString(connectionName);
        builder.Services.AddDbContext<DbContext, TContext>(options =>
            options.UseSqlServer(connectionString)); return builder;
    }

    /// <summary>
    /// Adds a PostgreSQL DbContext with automatic migrations in Development environment.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type to register</typeparam>
    /// <param name="builder">The host application builder instance</param>
    /// <param name="connectionName">The name of the connection string in configuration</param>
    /// <returns>The builder instance for method chaining</returns>
    public static IHostApplicationBuilder AddPostgreSqlPersistence<TContext>(
        this IHostApplicationBuilder builder,
        string connectionName)
        where TContext : DbContext
    {
        string? connectionString = builder.Configuration.GetConnectionString(connectionName);
        builder.Services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(TContext).Assembly.GetName().Name);
                npgsqlOptions.EnableRetryOnFailure(3);
            });
            options.UseSnakeCaseNamingConvention();
        });
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddHostedService<DatabaseMigrator<TContext>>();
        }
        return builder;
    }

    /// <summary>
    /// Applies database migrations automatically only when running locally in Development environment with Dapr sidecar.
    /// This method ensures safe migration execution by validating the environment and configuration before proceeding.
    /// </summary>
    /// <param name="app">The web application instance</param>
    /// <returns>The web application instance for method chaining</returns>
    public static WebApplication ApplyDatabaseMigrations(this WebApplication app)
    {
        if (ShouldApplyMigrations(app))
        {
            using IServiceScope scope = app.Services.CreateScope();
            IEnumerable<DbContext> dbContexts = scope.ServiceProvider.GetServices<DbContext>();

            foreach (DbContext context in dbContexts)
            {
                try
                {
                    context.Database.Migrate();
                    app.Logger.LogInformation("Database migrations applied successfully for {ContextType}",
                        context.GetType().Name);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "An error occurred while applying database migrations for {ContextType}",
                        context.GetType().Name);
                    throw;
                }
            }
        }
        else
        {
            app.Logger.LogInformation("Database migrations skipped - not running in local development with Dapr sidecar");
        }
        return app;
    }

    /// <summary>
    /// Determines if database migrations should be applied based on environment and Dapr configuration.
    /// Validates multiple safety conditions to ensure migrations only run in appropriate local development scenarios.
    /// </summary>
    /// <param name="app">The web application instance used for environment and configuration checks</param>
    /// <returns>True if migrations should be applied, false otherwise</returns>
    private static bool ShouldApplyMigrations(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.Logger.LogDebug("Migrations skipped: Not in Development environment");
            return false;
        }
        IConfiguration configuration = app.Configuration;
        string? daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
        string? daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT");
        if (string.IsNullOrEmpty(daprHttpPort) && string.IsNullOrEmpty(daprGrpcPort))
        {
            app.Logger.LogDebug("Migrations skipped: DAPR_HTTP_PORT and DAPR_GRPC_PORT environment variables not found");
            return false;
        }
        string? isContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
        string? isKubernetes = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
        if (!string.IsNullOrEmpty(isContainer) || !string.IsNullOrEmpty(isKubernetes))
        {
            app.Logger.LogWarning("Migrations skipped: Running in containerized environment (DOTNET_RUNNING_IN_CONTAINER={Container}, KUBERNETES_SERVICE_HOST={Kubernetes})",
                isContainer, isKubernetes);
            return false;
        }
        bool? migrationsEnabled = configuration.GetValue<bool?>("Database:AutoMigrations:Enabled");
        if (migrationsEnabled.HasValue && !migrationsEnabled.Value)
        {
            app.Logger.LogInformation("Migrations skipped: Explicitly disabled via Database:AutoMigrations:Enabled configuration");
            return false;
        }
        app.Logger.LogInformation("Database migrations will be applied - running in Development with Dapr sidecar (DAPR_HTTP_PORT={DaprHttpPort}, DAPR_GRPC_PORT={DaprGrpcPort})",
            daprHttpPort, daprGrpcPort);

        return true;
    }
}
