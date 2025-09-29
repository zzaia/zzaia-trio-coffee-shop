using Zzaia.CoffeeShop.AppHost.Applications.Order;
using Zzaia.CoffeeShop.AppHost.Applications.Wasm;
using Zzaia.CoffeeShop.AppHost.Applications.BFF;
using Zzaia.CoffeeShop.AppHost.Applications.LLM;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Extensions.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

string keyVaultName = builder.Configuration["Azure:KeyVault:Name"] ?? throw new ArgumentNullException("Azure:KeyVault:Name");
string keyVaultUri = configuration["Azure:KeyVault:Uri"] ?? throw new ArgumentNullException("Azure:KeyVault:Uri");
DefaultAzureCredentialOptions defaultAzureCredentialOptions = new() { ExcludeEnvironmentCredential = false };
configuration.AddAzureKeyVault(
    new Uri(keyVaultUri),
    new DefaultAzureCredential(defaultAzureCredentialOptions),
    new AzureKeyVaultConfigurationOptions()
    {
        ReloadInterval = new TimeSpan(0, 10, 0)
    });

IResourceBuilder<ParameterResource> dbPassword = builder.AddParameter("DatabasePassword", secret: true);
if (builder.Environment.IsDevelopment() && string.IsNullOrEmpty(dbPassword.Resource.Value))
    throw new InvalidOperationException("A password for the local SQL Server container is not configured.");
IResourceBuilder<SqlServerServerResource> database = builder
    .AddSqlServer("sql-coffee-shop-local", dbPassword, port: 65536)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithVolume("zzaia-coffee-shop-data", "/var/opt/mssql");

builder.AddOrderApplication(keyVaultName, database);
builder.AddBFFApplication(keyVaultName);
builder.AddWasmApplication();
builder.AddLLMApplication(keyVaultName);

builder.Build().Run();
