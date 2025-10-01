using Zzaia.CoffeeShop.AppHost.Applications.Order;
using Zzaia.CoffeeShop.AppHost.Applications.Wasm;
using Zzaia.CoffeeShop.AppHost.Applications.BFF;
using Zzaia.CoffeeShop.AppHost.Applications.LLM;
using Zzaia.CoffeeShop.AppHost.Applications.Identity;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Infrastructure Services
// PostgreSQL 15 Database
IResourceBuilder<ParameterResource> dbPassword = builder.AddParameter("DatabasePassword", secret: true);
if (builder.Environment.IsDevelopment() && string.IsNullOrEmpty(dbPassword.Resource.Value))
    throw new InvalidOperationException("A password for the PostgreSQL container is not configured.");

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres-coffee-shop", dbPassword, port: 5432)
    .WithImage("postgres", "15-alpine")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithVolume("zzaia-coffee-shop-postgres-data", "/var/lib/postgresql/data")
    .WithEnvironment("POSTGRES_DB", "coffeeshop")
    .WithHealthCheck();

IResourceBuilder<PostgresDatabaseResource> database = postgres.AddDatabase("CoffeeShopDb");

// Redis 7 for Caching
IResourceBuilder<RedisResource> redis = builder
    .AddRedis("redis-coffee-shop", port: 6379)
    .WithImage("redis", "7-alpine")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithVolume("zzaia-coffee-shop-redis-data", "/data")
    .WithHealthCheck();

// Kafka for Event Streaming
IResourceBuilder<KafkaServerResource> kafka = builder
    .AddKafka("kafka-coffee-shop", port: 9092)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithVolume("zzaia-coffee-shop-kafka-data", "/var/lib/kafka/data")
    .WithHealthCheck();

// HashiCorp Vault for Secret Management (Development only)
IResourceBuilder<ContainerResource>? vault = null;
if (builder.Environment.IsDevelopment())
{
    vault = builder.AddContainer("vault-coffee-shop", "hashicorp/vault", "1.15")
        .WithHttpEndpoint(port: 8200, targetPort: 8200, name: "http")
        .WithEnvironment("VAULT_DEV_ROOT_TOKEN_ID", "dev-token")
        .WithEnvironment("VAULT_DEV_LISTEN_ADDRESS", "0.0.0.0:8200")
        .WithArgs("server", "-dev")
        .WithLifetime(ContainerLifetime.Persistent);
}

// Application Services
builder.AddIdentityApplication(database, redis, kafka);
builder.AddOrderApplication(database, redis, kafka);
builder.AddBFFApplication(redis, kafka);
builder.AddWasmApplication();
builder.AddLLMApplication();

builder.Build().Run();
