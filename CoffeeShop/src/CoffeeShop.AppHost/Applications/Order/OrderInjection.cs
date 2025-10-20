using CommunityToolkit.Aspire.Hosting.Dapr;
using Zzaia.CoffeeShop.AppHost.Helpers;

namespace Zzaia.CoffeeShop.AppHost.Applications.Order;

/// <summary>
/// Extension methods for adding Order service to the distributed application
/// </summary>
public static class OrderInjection
{
    /// <summary>
    /// Adds the Order service with Dapr sidecar, PostgreSQL database, Redis cache, and Kafka messaging
    /// </summary>
    /// <param name="builder">The distributed application builder</param>
    /// <param name="database">PostgreSQL database resource</param>
    /// <param name="redis">Redis cache resource</param>
    /// <param name="kafka">Kafka messaging resource</param>
    /// <returns>The distributed application builder for chaining</returns>
    public static IDistributedApplicationBuilder AddOrderApplication(
        this IDistributedApplicationBuilder builder,
        string namespaceName,
        IResourceBuilder<PostgresServerResource> sqlServer,
        IResourceBuilder<RedisResource> redis,
        IResourceBuilder<KafkaServerResource> kafka)
    {
        string appName = $"{namespaceName}-order";
        string resourcesPath = @"Applications\Order\Dapr";
        FileHelper.ReplaceVariablesInYamlFiles([resourcesPath], new Dictionary<string, string>
        {
            ["__NAMESPACE__"] = namespaceName,
            ["__DAPR_APP_ID__"] = appName,
        });

        string fullDir = FileHelper.CombineCrossPlatformPath(AppContext.BaseDirectory, resourcesPath);
        IResourceBuilder<PostgresDatabaseResource> dbOrder = sqlServer.AddDatabase("db-order");
        builder.AddProject<Projects.CoffeeShop_Order>(appName)
               .WithDaprSidecar(new DaprSidecarOptions
               {
                   AppId = appName,
                   DaprHttpPort = 3502,
                   DaprGrpcPort = 50002,
                   MetricsPort = 9094,
                   ResourcesPaths = [fullDir],
               })
               .WithReference(dbOrder)
               .WithReference(redis)
               .WithReference(kafka)
               .WaitFor(dbOrder)
               .WaitFor(redis)
               .WaitFor(kafka);

        return builder;
    }
}