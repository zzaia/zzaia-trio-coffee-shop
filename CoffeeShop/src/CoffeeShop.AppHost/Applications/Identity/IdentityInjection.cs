using CommunityToolkit.Aspire.Hosting.Dapr;
using Zzaia.CoffeeShop.AppHost.Helpers;

namespace Zzaia.CoffeeShop.AppHost.Applications.Identity;

/// <summary>
/// Extension methods for adding Identity service to the distributed application
/// </summary>
public static class IdentityInjection
{
    /// <summary>
    /// Adds the Identity service with Dapr sidecar, PostgreSQL database, Redis cache, and Kafka messaging
    /// </summary>
    /// <param name="builder">The distributed application builder</param>
    /// <param name="database">PostgreSQL database resource</param>
    /// <param name="redis">Redis cache resource</param>
    /// <param name="kafka">Kafka messaging resource</param>
    /// <returns>The distributed application builder for chaining</returns>
    public static IDistributedApplicationBuilder AddIdentityApplication(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<PostgresDatabaseResource> database,
        IResourceBuilder<RedisResource> redis,
        IResourceBuilder<KafkaServerResource> kafka)
    {
        string namespaceName = "coffee-shop";
        string appName = $"{namespaceName}-identity";
        string resourcesPath = @"Applications\Identity\Dapr";
        FileHelper.ReplaceVariablesInYamlFiles([resourcesPath], new Dictionary<string, string>
        {
            ["__NAMESPACE__"] = namespaceName,
            ["__DAPR_APP_ID__"] = appName,
        });
        string fullDir = FileHelper.CombineCrossPlatformPath(AppContext.BaseDirectory, resourcesPath);

        builder.AddProject<Projects.CoffeeShop_Identity>(appName)
               .WithDaprSidecar(new DaprSidecarOptions
               {
                   AppId = appName,
                   DaprHttpPort = 3502,
                   DaprGrpcPort = 50003,
                   MetricsPort = 9093,
                   ResourcesPaths = [fullDir],
               })
               .WithReference(database)
               .WithReference(redis)
               .WithReference(kafka)
               .WaitFor(database)
               .WaitFor(redis)
               .WaitFor(kafka);

        return builder;
    }
}
