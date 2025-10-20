namespace Zzaia.CoffeeShop.AppHost.Applications.Wasm;

/// <summary>
/// Extension methods for adding Blazor WebAssembly service to the distributed application
/// </summary>
public static class WasmInjection
{
    /// <summary>
    /// Adds the Blazor WebAssembly frontend application
    /// </summary>
    /// <param name="builder">The distributed application builder</param>
    /// <returns>The distributed application builder for chaining</returns>
    public static IDistributedApplicationBuilder AddWasmApplication(
        this IDistributedApplicationBuilder builder)
    {
        string appName = "coffee-shop-wasm";
        builder.AddProject<Projects.CoffeeShop_Wasm>(appName);

        return builder;
    }
}