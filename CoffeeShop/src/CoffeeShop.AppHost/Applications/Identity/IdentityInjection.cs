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
    /// <param name="sqlServer">PostgreSQL database resource</param>
    /// <param name="redis">Redis cache resource</param>
    /// <param name="kafka">Kafka messaging resource</param>
    /// <returns>The distributed application builder for chaining</returns>
    public static IDistributedApplicationBuilder AddIdentityApplication(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<PostgresServerResource> sqlServer,
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
        IResourceBuilder<PostgresDatabaseResource> dbIdentity = sqlServer.AddDatabase("db-identity");
        builder.AddProject<Projects.CoffeeShop_Identity>(appName)
               .WithDaprSidecar(new DaprSidecarOptions
               {
                   AppId = appName,
                   DaprHttpPort = 3501,
                   DaprGrpcPort = 50001,
                   MetricsPort = 9091,
                   ResourcesPaths = [fullDir],
               })
               .WithReference(dbIdentity)
               .WithReference(redis)
               .WithReference(kafka)
               .WaitFor(dbIdentity)
               .WaitFor(redis)
               .WaitFor(kafka);

        return builder;
    }
}
