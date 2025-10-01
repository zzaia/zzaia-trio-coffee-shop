# Coffee Shop Order Management - System Architecture Overview

This is an overview architecture of a coffee sho ordering system designed as microservice pattern.

## System Architecture Diagram

```mermaid
C4Context
    Person(user, "User", "Coffee shop customer")

    System_Boundary(frontend, "Frontend") {
        Container(wasm, "CoffeeShop.Wasm", "Blazor WebAssembly", "Client application")
    }

    System_Boundary(api, "API Gateway") {
        Container(bff, "CoffeeShop.BFF", "ASP.NET Core", "Ingress gateway and routing")
    }

    System_Boundary(services, "Services") {
        Container(identity, "CoffeeShop.Identity", "ASP.NET Core", "Authentication and authorization")
        Container(order, "CoffeeShop.Order", "ASP.NET Core", "Order management")
    }

    System_Boundary(infrastructure, "Infrastructure") {
        ContainerDb(postgres, "PostgreSQL", "Database", "User and order data")
        ContainerDb(redis, "Redis", "Cache", "Token caching")
        Container(vault, "HashiCorp Vault", "Secrets", "Secret management")
        Container(dapr, "Dapr", "Runtime", "Service communication")
    }

    System_Ext(payment, "Payment Service", "External payment processing")
    System_Ext(notification, "Notification Service", "External notifications")

    Rel(user, wasm, "Uses")
    Rel(wasm, bff, "Authenticated requests", "HTTPS")
    Rel(bff, identity, "User authentication", "OAuth2/OIDC")
    Rel(bff, order, "Authenticated order requests", "HTTPS")
    Rel(order, identity, "Service authentication", "client_credentials")
    Rel(order, payment, "Payment processing", "HTTPS")
    Rel(order, notification, "Send notifications", "HTTPS")

    Rel(identity, postgres, "Read/Write user data", "SQL")
    Rel(order, postgres, "Read/Write order data", "SQL")
    Rel(identity, redis, "Cache tokens", "Redis Protocol")

    Rel(bff, vault, "Get secrets", "HTTPS")
    Rel(identity, vault, "Get secrets", "HTTPS")
    Rel(order, vault, "Get secrets", "HTTPS")

    Rel(bff, dapr, "Service invocation")
    Rel(identity, dapr, "Service invocation")
    Rel(order, dapr, "Service invocation")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="2")
```

## Application Responsibilities

### CoffeeShop.BFF (Ingress Gateway)
- Acts as ingress gateway for frontend requests
- Implements request routing to backend services
- Handles user token validation and forwarding to client credential authentication
- Handling rate limiting, and circuit breaking
- Aggregate data for front-end purpose 
- Provides secure entry point for frontend
- Provide flexible data access for front-end customized end-points 
- Can cache common responses to improve performance

### CoffeeShop.SecretStore
- Self-hosted secret management using HashiCorp Vault
- Centralized secret storage and retrieval
- Secure initialization and unsealing strategies
- Integrates with Dapr Secret Store Component

### CoffeeShop.Order
- Authenticates with CoffeeShop.Identity using client_credentials grant
- Machine-to-machine authentication for BFF requests
- Independent domain service with dedicated database
- Handles payment and notification integrations

### CoffeeShop.Identity
- Handles all authentication flows, user authentication and client credentials
- Internal persistency of user information side-by-side token management
- Can be used as Dapr internal service 

## Infrastructure Overview 
- User and application authentication through Identity service
- Lightweight BFF ingress layer
- Centralized secret management via HashiCorp Vault
- Cloud-native architecture with Dapr and kubernetes
- Redis as caching database
- PostgresSQL as persistence database
- Kafka as notification system

## Development
- ASPIRE for local cluster development
