# Coffee Shop Order Management - System Architecture Overview

## System Architecture Diagram

```mermaid
C4Context
    title Coffee Shop System Architecture

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

## Key Architectural Changes
- **BFF as Ingress Gateway**: All frontend requests route through CoffeeShop.BFF for authentication and routing only
- **Service-to-Service Authentication**: CoffeeShop.Order uses client_credentials flow with CoffeeShop.Identity
- **Replaced Azure Key Vault with HashiCorp Vault**
- **Enhanced Secret Management with Dapr Secret Store Component**

## Updated Application Responsibilities

### CoffeeShop.BFF (Ingress Gateway)
- Acts as ingress gateway for frontend requests
- Implements request routing to backend services
- Handles user token validation and forwarding
- No data aggregation - pure routing layer
- Provides secure entry point for frontend

### CoffeeShop.SecretStore (New Component)
- Self-hosted secret management using HashiCorp Vault
- Centralized secret storage and retrieval
- Secure initialization and unsealing strategies
- Integrates with Dapr Secret Store Component

### CoffeeShop.Order (Service Authentication)
- Authenticates with CoffeeShop.Identity using client_credentials grant
- Machine-to-machine authentication for BFF requests
- Independent service with dedicated database
- Handles payment and notification integrations

## Technology Stack Updates
- **Secret Management**: HashiCorp Vault (replaced Azure Key Vault)
- **Ingress Pattern**: Implemented with lightweight BFF
- **Service Communication**: Dapr integration with OAuth2 client_credentials
- **Development Orchestration**: .NET Aspire AppHost (development environment only)

## Security Architecture Enhancements
- Centralized secret management via HashiCorp Vault
- User authentication via OAuth2/OIDC through Identity service
- Service-to-service authentication via client_credentials flow
- BFF as ingress point - no direct service exposure
- Request validation at gateway layer

## Communication Patterns
- BFF acts as ingress and routing layer only
- User authentication via OAuth2/OIDC
- Service-to-service authentication via client_credentials
- Dapr for service invocation
- Distributed tracing capabilities

## Infrastructure Scalability
- Lightweight BFF ingress layer
- Flexible secret management via HashiCorp Vault
- Cloud-native architecture with Dapr and kubernetes