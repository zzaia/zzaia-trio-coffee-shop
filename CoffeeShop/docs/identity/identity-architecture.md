# CoffeeShop.Identity - Authentication and Authorization Architecture

This application is responsible to all users and applications authentication, authorization and identification.

## C4 Model - Container Diagram

```mermaid
C4Container
    title Container Diagram - CoffeeShop.Identity Service

    Person(user, "User", "Customer or Manager")

    Container_Boundary(identity, "CoffeeShop.Identity Service") {
        Container(api, "Identity API", "ASP.NET Core Web API", "Exposes authentication and user management endpoints")
        Container(openIddict, "OpenIddict", ".NET Library", "OAuth2/OIDC provider for token generation")
        Container(authService, "Authentication Service", "C# Service Layer", "User validation and credential management")
        Container(userService, "User Management Service", "C# Service Layer", "User profile management")
        Container(tokenService, "Token Service", "C# Service Layer", "JWT generation and refresh token management")
        Container(eventPublisher, "Event Publisher", "C# Service Layer", "User synchronization event publishing")
    }

    ContainerDb(postgres, "PostgreSQL Database", "PostgreSQL 15", "Stores users, refresh tokens, OpenIddict data")
    ContainerDb(redis, "Redis Cache", "Redis 7", "Caches active tokens and user sessions")

    Container_Ext(dapr, "Dapr Sidecar", "Service mesh and pub/sub")
    Container_Ext(kafka, "Kafka", "Event streaming and notifications")
    System_Ext(clients, "External Clients", "BFF, Order Service")

    Rel(user, api, "Makes requests", "HTTPS/JSON")
    Rel(clients, api, "Authenticates/Validates", "HTTPS/JSON")

    Rel(api, openIddict, "Uses", "In-process")
    Rel(api, authService, "Delegates to", "In-process")
    Rel(api, userService, "Delegates to", "In-process")

    Rel(authService, tokenService, "Uses", "In-process")
    Rel(userService, eventPublisher, "Publishes events", "In-process")

    Rel(openIddict, postgres, "Reads configuration", "EF Core/Npgsql")
    Rel(authService, postgres, "Validates credentials", "EF Core/Npgsql")
    Rel(userService, postgres, "Manages users", "EF Core/Npgsql")

    Rel(tokenService, redis, "Caches tokens", "StackExchange.Redis")
    Rel(authService, redis, "Validates cached tokens", "StackExchange.Redis")

    Rel(eventPublisher, dapr, "Publishes events", "Dapr SDK")
    Rel(dapr, kafka, "Pub/Sub", "Kafka Protocol")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```
## Authentication Flow Integration

### User Registration Flow
```mermaid
sequenceDiagram
    participant Client
    participant Identity
    participant Postgres
    participant Dapr
    participant Kafka
    participant OrderService

    Client->>Identity: POST /register
    Identity->>Postgres: Create User (email_verified=false, role="customer")
    Postgres-->>Identity: User Created
    Identity->>Dapr: Publish user.created Event
    Dapr->>Kafka: Publish to user.created Topic
    Kafka-->>OrderService: Subscribe to user.created Event
    Identity-->>Client: 201 Created (email verification required)
```

### Login Flow with Token Generation (OpenIddict)
```mermaid
sequenceDiagram
    participant Client
    participant Identity
    participant Postgres
    participant Redis

    Client->>Identity: POST /connect/token (username, password)
    Identity->>Postgres: Validate Credentials
    Postgres-->>Identity: User (id, username, email, role)
    Identity->>Identity: Generate JWT + Refresh Token (OpenIddict)
    Identity->>Redis: Cache Refresh Token
    Identity->>Postgres: Create RefreshToken Record
    Identity->>Postgres: Update last_login_at
    Identity-->>Client: {access_token, refresh_token, token_type, expires_in}
```

### Token Refresh Flow
```mermaid
sequenceDiagram
    participant Client
    participant Identity
    participant Redis
    participant Postgres

    Client->>Identity: POST /connect/token (grant_type=refresh_token)
    Identity->>Redis: Validate Refresh Token
    alt Token Valid in Cache
        Redis-->>Identity: Token Data
    else Token Not in Cache
        Identity->>Postgres: Query RefreshToken
        Postgres-->>Identity: Token Data
    end
    Identity->>Identity: Generate New JWT + Refresh Token
    Identity->>Postgres: Revoke Old Token
    Identity->>Postgres: Create New RefreshToken
    Identity->>Redis: Cache New Token
    Identity-->>Client: {access_token, refresh_token, token_type, expires_in}
```

### Application Authentication Flow (Client Credentials)
```mermaid
sequenceDiagram
    participant Application as Service Application (Order/BFF)
    participant Identity as CoffeeShop.Identity
    participant Postgres as PostgreSQL
    participant Redis as Redis Cache

    Application->>Identity: POST /connect/token (grant_type=client_credentials, client_id, client_secret)
    Identity->>Postgres: Validate Client Credentials
    Postgres-->>Identity: Application (id, client_id, name, is_active)
    Identity->>Identity: Generate JWT (OpenIddict)
    Identity->>Redis: Cache Application Token
    Identity->>Postgres: Create ApplicationToken Record
    Identity-->>Application: {access_token, token_type, expires_in}
```

---

## Clean Architecture Layer Structure

```
CoffeeShop.Identity/
├── Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── RefreshToken.cs
│   │   ├── Application.cs
│   │   └── ApplicationToken.cs
│   ├── ValueObjects/
│   │   ├── Email.cs
│   │   └── Password.cs
│   ├── Enums/
│   │   └── UserRole.cs
│   └── Events/
│       ├── UserCreatedEvent.cs
│       └── UserUpdatedEvent.cs
├── Application/
│   ├── Commands/
│   │   ├── RegisterUserCommand.cs
│   │   ├── LoginCommand.cs
│   │   ├── RefreshTokenCommand.cs
│   │   └── AuthenticateApplicationCommand.cs
│   ├── Queries/
│   │   ├── GetUserByIdQuery.cs
│   │   ├── ValidateTokenQuery.cs
│   │   └── ValidateApplicationTokenQuery.cs
│   ├── DTOs/
│   │   ├── UserDto.cs
│   │   └── TokenResponseDto.cs
│   ├── Interfaces/
│   │   ├── IUserRepository.cs
│   │   ├── IApplicationRepository.cs
│   │   └── ITokenService.cs
│   └── Behaviors/
│       ├── ValidationBehavior.cs
│       └── LoggingBehavior.cs
├── Infrastructure/
│   ├── Persistence/
│   │   ├── IdentityDbContext.cs
│   │   ├── UserRepository.cs
│   │   └── Migrations/
│   ├── Identity/
│   │   ├── OpenIddictConfiguration.cs
│   │   └── TokenService.cs
│   ├── Caching/
│   │   └── RedisCacheService.cs
│   ├── Events/
│   │   └── EventPublisher.cs
│   └── ExternalServices/
│       └── MessageQueueService.cs
└── API/
    ├── Controllers/
    │   ├── AuthController.cs
    │   └── UserController.cs
    ├── Middleware/
    │   ├── RateLimitingMiddleware.cs
    │   └── ExceptionHandlerMiddleware.cs
    └── Program.cs
```

### Token Generation Flow
1. **User Login (ROPC Flow)**
   - Client sends username/password
   - Validate credentials against PostgreSQL
   - Generate JWT with user claims (role, username, email)
   - Create refresh token
   - Store tokens in Redis
   - Return tokens to client

2. **Token Refresh**
   - Client sends refresh token
   - Validate refresh token
   - Generate new access token
   - Rotate refresh token
   - Return new tokens

3. **Service-to-Service (Client Credentials)**
   - Service sends client_id/client_secret
   - Generate access token with service claims
   - Return short-lived token (5 minutes)

## Key Design Decisions

## Exception Handling
- Use of Exception handler middleware to generate problem details responses
- Return problem details with validation errors from fluent validation exceptions

## Event driven design
- Use of CQRS with mediator pattern
- Use of pipeline behavior in mediator for validation and logging
- Publish notifications to the internal notification system using Dapr
- Publish user events only to internal notification system

## Logging
- Avoid logging throughout the code
- Use the logging pipeline behavior pattern to log commands, responses and exceptions  

## Authentication & Authorization 
- OAuth2 and OpenID Connect compliant
- Use the OpenIddict to handle all authentication and authorization service 
- Supports authentication flows:
  - Resource Owner Password Credentials (ROPC) for SPA login
  - Client Credentials for service-to-service
- Token lifetimes:
  - Access token: 15 minutes
  - Refresh token: 30 days
- Simple role-based claims (Customer, Manager)

## Exposed end-points
- Rest API exposed using the OpenIddict 
- Exposed end-points using the Dapr

## Mapping
- Classes must be mapped using the Mapster 
- Implement the profile pattern injected in the DI

## Internal service integration
- Order service handles payment processing internally
- Circuit breaker for internal service calls
- Retry mechanisms with exponential backoff
- Timeout configurations per service

## Secret Store
- Use of the dapr client secret-store integration
- Retrieve all secrets during application initialization and add to DI, scoped by application

## Validation Rules
- Use of fluent validation defined in the DI
- Use the validation pipeline behavior to validate commands, and responses

## Persistency
- Use of Redis to cache
- Redis for active refresh tokens (TTL-based)
- User profile cache (5-minute TTL)
- Token storage with sliding expiration
- Use of PostgreSQL to persist all domain models
- Use Entity Framework for database operations
- Use the ServiceDefaults persistence extension to automatic apply migrations 
- Use Entity Framework to insert the product data during the migrations
- Optimized indexes for common queries
- Asynchronous queries for all database operations

## Security Best Practices

### Password Policy
- Minimum length: 8 characters
- Require: Uppercase, lowercase, digit, special character
- Complexity validation on client and server
- BCrypt cost factor: 12

### Rate Limiting
- Login attempts: 5 per 15 minutes per IP
- Registration: 3 per hour per IP
- Token refresh: 10 per minute per user
- Refresh Token rotation

### Encryption
- Passwords: BCrypt with cost factor 12
- Refresh tokens: SHA-256 hashing
- In transit: TLS 1.3 for all communications

## Recommended Tools
- .NET 9
- ASP.NET Core
- MediatR
- FluentValidation
- Entity Framework Core
- Npgsql
- Mapster
- Dapr
- Docker
- Kubernetes
- Redis
- Kafka
- OpenIddict

### OpenIddict Setup Example
```csharp
services.AddOpenIddict()
    .AddCore(options => {
        options.UseEntityFrameworkCore()
            .UseDbContext<IdentityDbContext>();
    })
    .AddServer(options => {
        options.SetTokenEndpointUris("/connect/token")
               .SetUserinfoEndpointUris("/connect/userinfo");

        options.AllowPasswordFlow()
               .AllowRefreshTokenFlow()
               .AllowClientCredentialsFlow();

        options.RegisterScopes("openid", "profile", "email", "roles");

        options.AddEphemeralEncryptionKey()
               .AddEphemeralSigningKey();
    })
    .AddValidation(options => {
        options.UseLocalServer();
        options.UseAspNetCore();
    });
```