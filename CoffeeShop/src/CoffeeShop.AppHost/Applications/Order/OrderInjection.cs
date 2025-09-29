using CommunityToolkit.Aspire.Hosting.Dapr;
using Zzaia.CoffeeShop.AppHost.Helpers;

namespace Zzaia.CoffeeShop.AppHost.Applications.Order;

/// <summary>
/// Extension methods for adding Order service to the distributed application
/// </summary>
public static class OrderInjection
{
    /// <summary>
    /// Adds the Order service with Dapr sidecar and SQL database configuration
    /// </summary>
    /// <param name="builder">The distributed application builder</param>
    /// <param name="keyVaultName">Azure Key Vault name for secrets</param>
    /// <param name="sqlDatabase">SQL Server database resource</param>
    /// <returns>The distributed application builder for chaining</returns>
    public static IDistributedApplicationBuilder AddOrderApplication(
        this IDistributedApplicationBuilder builder,
        string keyVaultName,
        IResourceBuilder<SqlServerServerResource> sqlDatabase)
    {
        string namespaceName = "coffee-shop";
        string appName = $"{namespaceName}-order";
        string resourcesPath = @"Applications\Order\Dapr";
        FileHelper.ReplaceVariablesInYamlFiles([resourcesPath], new Dictionary<string, string>
        {
            ["__VAULT_NAME__"] = keyVaultName,
            ["__NAMESPACE__"] = namespaceName,
            ["__DAPR_APP_ID__"] = appName,
        });
        string fullDir = FileHelper.CombineCrossPlatformPath(AppContext.BaseDirectory, resourcesPath);
        IResourceBuilder<SqlServerDatabaseResource> orderDatabase = sqlDatabase.AddDatabase("CoffeeShopDb", "db-order");
        builder.AddProject<Projects.CoffeeShop_Order>(appName)
               .WithDaprSidecar(new DaprSidecarOptions
               {
                   AppId = appName,
                   DaprHttpPort = 3500,
                   DaprGrpcPort = 50001,
                   MetricsPort = 9091,
                   ResourcesPaths = [fullDir],
               })
               .WithReference(orderDatabase)
               .WaitFor(orderDatabase);

        return builder;
    }
}