using CommunityToolkit.Aspire.Hosting.Dapr;
using Zzaia.CoffeeShop.AppHost.Helpers;

namespace Zzaia.CoffeeShop.AppHost.Applications.BFF;

/// <summary>
/// Extension methods for adding BFF service to the distributed application
/// </summary>
public static class BFFInjection
{
    /// <summary>
    /// Adds the BFF (Backend for Frontend) service with Dapr sidecar, Redis cache, and Kafka messaging
    /// </summary>
    /// <param name="builder">The distributed application builder</param>
    /// <param name="redis">Redis cache resource</param>
    /// <param name="kafka">Kafka messaging resource</param>
    /// <returns>The distributed application builder for chaining</returns>
    public static IDistributedApplicationBuilder AddBFFApplication(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<RedisResource> redis,
        IResourceBuilder<KafkaServerResource> kafka)
    {
        string namespaceName = "coffee-shop";
        string appName = $"{namespaceName}-bff";
        string resourcesPath = @"Applications\BFF\Dapr";
        FileHelper.ReplaceVariablesInYamlFiles([resourcesPath], new Dictionary<string, string>
        {
            ["__NAMESPACE__"] = namespaceName,
            ["__DAPR_APP_ID__"] = appName,
        });
        string fullDir = FileHelper.CombineCrossPlatformPath(AppContext.BaseDirectory, resourcesPath);

        builder.AddProject<Projects.CoffeeShop_BFF>(appName)
               .WithDaprSidecar(new DaprSidecarOptions
               {
                   AppId = appName,
                   DaprHttpPort = 3501,
                   DaprGrpcPort = 50002,
                   MetricsPort = 9092,
                   ResourcesPaths = [fullDir],
               })
               .WithReference(redis)
               .WithReference(kafka)
               .WaitFor(redis)
               .WaitFor(kafka);

        return builder;
    }
}