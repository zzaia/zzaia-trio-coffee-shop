using CommunityToolkit.Aspire.Hosting.Dapr;
using Zzaia.CoffeeShop.AppHost.Helpers;

namespace Zzaia.CoffeeShop.AppHost.Applications.LLM;

/// <summary>
/// Extension methods for adding LLM service to the distributed application
/// </summary>
public static class LLMInjection
{
    /// <summary>
    /// Adds the LLM (Large Language Model) Python FastAPI service with Dapr sidecar
    /// </summary>
    /// <param name="builder">The distributed application builder</param>
    /// <returns>The distributed application builder for chaining</returns>
    public static IDistributedApplicationBuilder AddLLMApplication(
        this IDistributedApplicationBuilder builder)
    {
        string namespaceName = "coffee-shop";
        string appName = $"{namespaceName}-llm";
        string resourcesPath = @"Applications\LLM\Dapr";
        FileHelper.ReplaceVariablesInYamlFiles([resourcesPath], new Dictionary<string, string>
        {
            ["__NAMESPACE__"] = namespaceName,
            ["__DAPR_APP_ID__"] = appName,
        });
        string fullDir = FileHelper.CombineCrossPlatformPath(AppContext.BaseDirectory, resourcesPath);

        builder.AddContainer(appName, "coffee-shop-llm")
               .WithDockerfile("../CoffeeShop.LLM")
               .WithHttpEndpoint(port: 8000, targetPort: 8000)
               .WithDaprSidecar(new DaprSidecarOptions
               {
                   AppId = appName,
                   DaprHttpPort = 3502,
                   DaprGrpcPort = 50003,
                   MetricsPort = 9093,
                   ResourcesPaths = [fullDir],
               });

        return builder;
    }
}