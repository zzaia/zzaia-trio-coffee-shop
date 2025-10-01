# Coffee Shop Order Management System

> **Portfolio Project**: Enterprise-grade microservices architecture showcasing Solution Architecture and Full-Stack Development capabilities.

[![.NET](https://img.shields.io/badge/.NET-9-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%7C%20CQRS%20%7C%20Event--Driven-blue)]()
[![Infrastructure](https://img.shields.io/badge/Infrastructure-Dapr%20%7C%20Kafka%20%7C%20Kubernetes-orange)]()
[![AI-Assisted](https://img.shields.io/badge/AI--Assisted-Claude%20Code-purple)](https://github.com/raphael-pizzaia/zzaia-agentic-workspace)

---

## 🎯 Project Overview

Production-ready coffee shop order management system built with modern microservices architecture, demonstrating enterprise-level design patterns, cloud-native deployment, and event-driven architecture.

**Key Highlights**:
- ✅ Clean Architecture with strict layer separation
- ✅ CQRS pattern with MediatR
- ✅ Event-driven architecture using Kafka
- ✅ OAuth2/OIDC authentication with OpenIddict
- ✅ Service mesh with Dapr
- ✅ Kubernetes-ready deployment
- ✅ Comprehensive testing strategy (unit, integration, load)

---

## 🏗️ System Architecture

### Microservices
- **Identity Service**: OAuth2/OIDC authentication, user management, role-based authorization
- **Order Service**: Order management, payment integration, external notifications
- **BFF (Backend for Frontend)**: GraphQL API gateway, real-time subscriptions, request aggregation

### Infrastructure
- **Database**: PostgreSQL 15 (separate databases per service: `db-identity`, `db-order`)
- **Cache**: Redis 7 (shared instance for token caching and response caching)
- **Event Streaming**: Kafka (3 brokers, replication factor 3)
- **Service Mesh**: Dapr (service invocation, pub/sub, secret management)
- **Secret Management**: HashiCorp Vault
- **Orchestration**: Kubernetes with .NET Aspire + Aspire8

### Technology Stack
- **Backend**: .NET 9, ASP.NET Core Minimal API, OpenIddict, MediatR, FluentValidation, Mapster
- **Data**: Entity Framework Core, Npgsql, Redis (StackExchange.Redis)
- **Messaging**: Kafka via Dapr Pub/Sub
- **API**: GraphQL (HotChocolate), Swagger/OpenAPI
- **Resilience**: Polly (circuit breaker, retry, timeout)
- **Frontend**: Blazor WebAssembly (planned)
- **DevOps**: Docker, Kubernetes, .NET Aspire, Aspire8, GitHub Actions

---

## 📚 Documentation

Comprehensive documentation covering architecture, data models, implementation plans, and operational procedures.

### Architecture Documentation
- **[System Architecture Overview](./CoffeeShop/docs/architecture-overview.md)**: C4 diagrams, service responsibilities, infrastructure overview
- **[Identity Service Architecture](./CoffeeShop/docs/identity/identity-architecture.md)**: OAuth2/OIDC flows, OpenIddict configuration, authentication patterns
- **[Order Service Architecture](./CoffeeShop/docs/order/order-architecture.md)**: Order management, payment integration, status workflows
- **[BFF Architecture](./CoffeeShop/docs/bff/bff-architecture.md)**: GraphQL API gateway, role-based client credentials, real-time subscriptions

### Data Models
- **[Identity Data Models](./CoffeeShop/docs/identity/identity-data-models.md)**: Users, RefreshTokens, Applications, ApplicationTokens
- **[Order Data Models](./CoffeeShop/docs/order/order-data-models.md)**: Orders, OrderItems, Products, ProductVariations, Users (replicated)

### Event-Driven Architecture
- **[Event Notifications](./CoffeeShop/docs/event-notifications.md)**: Complete event catalog with schemas, Kafka configuration, idempotency patterns
  - User events: `user.created`, `user.updated`, `user.deleted`
  - Order events: `order.created`, `order.status.changed`

### Implementation Plans
- **[Master Implementation Plan](./CoffeeShop/docs/implementation-plan.md)**: 5-phase timeline (44-61 days), parallel execution strategy, team structure
- **[Identity Implementation Plan](./CoffeeShop/docs/identity/identity-implementation-plan.md)**: 13 phases (29-38 days), OpenIddict setup, event publishing
- **[Order Implementation Plan](./CoffeeShop/docs/order/order-implementation-plan.md)**: 11 phases (29-38 days), payment integration, compensation logic
- **[BFF Implementation Plan](./CoffeeShop/docs/bff/bff-implementation-plan.md)**: 11 phases (27-35 days), GraphQL schema, Dapr service invocation

---

## 🎓 Demonstrated Capabilities

### Solution Architecture
- **Microservices Design**: Service decomposition, bounded contexts, data isolation
- **Event-Driven Architecture**: Eventual consistency, pub/sub patterns, event sourcing
- **API Design**: RESTful APIs, GraphQL, real-time subscriptions (WebSockets)
- **Authentication & Authorization**: OAuth2/OIDC, client credentials flow, JWT validation, role-based access control
- **Resilience Patterns**: Circuit breaker, retry with exponential backoff, timeout, distributed locking
- **Compensation Logic**: Saga pattern for payment refunds on order creation failure

### Software Engineering
- **Clean Architecture**: Domain-driven design, layer separation, dependency inversion
- **CQRS Pattern**: Command/query segregation with MediatR
- **Domain Modeling**: Aggregates, value objects, domain events
- **Data Patterns**: Repository pattern, eventual consistency, denormalized read models
- **Validation**: FluentValidation with pipeline behaviors
- **Mapping**: Mapster with profile pattern

### Infrastructure & DevOps
- **Containerization**: Docker multi-stage builds, optimized images
- **Orchestration**: Kubernetes deployment, service mesh (Dapr)
- **Observability**: Structured logging (Serilog), health checks, distributed tracing (OpenTelemetry)
- **Secret Management**: HashiCorp Vault via Dapr
- **Local Development**: .NET Aspire for orchestration
- **CI/CD**: Automated testing, deployment pipelines

### Testing Strategy
- **Unit Testing**: Domain logic, command/query handlers (>80% coverage)
- **Integration Testing**: API endpoints with TestContainers, event flows
- **Load Testing**: Performance benchmarks, circuit breaker validation
- **Idempotency Testing**: Event deduplication, distributed systems reliability

---

## 🤖 AI-Assisted Development

This project showcases the integration of AI-powered development workflows using a **customized agentic system** built on Claude Code.

### Custom AI Workspace
- **Repository**: [zzaia-agentic-workspace](https://github.com/raphael-pizzaia/zzaia-agentic-workspace)
- **Toolset**: Claude Code with custom agents, slash commands, and output styles
- **Workflow**: Multi-repository management, git worktrees, automated task orchestration

---

## 📊 Project Statistics

- **Services**: 3 microservices (Identity, Order, BFF)
- **Documentation**: 10 comprehensive documents
- **Event Schemas**: 5 event types with complete JSON schemas
- **API Endpoints**: 5 REST + GraphQL schema
- **Test Coverage Target**: >80%
- **Database Strategy**: 2 PostgreSQL databases, 1 Redis instance
- **Infrastructure Components**: Kafka (3 brokers), Dapr, Vault, Kubernetes

---

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Docker Desktop
- .NET Aspire Workload
- Dapr CLI
- PostgreSQL 15
- Redis 7
- Apache Kafka

### Local Development
```bash
# Clone repository
git clone https://github.com/raphael-pizzaia/zzaia-trio-coffee-shop.git
cd zzaia-trio-coffee-shop

# Start infrastructure (PostgreSQL, Redis, Kafka, Vault)
dotnet run --project CoffeeShop.AppHost

# Run services
dotnet run --project CoffeeShop.Identity
dotnet run --project CoffeeShop.Order
dotnet run --project CoffeeShop.BFF
```

### Deployment
```bash
# Generate Kubernetes manifests
aspire8 generate

# Apply to cluster
kubectl apply -f manifests/
```

---

## 🔮 Future Development

### Planned Features
- **Blazor WebAssembly Frontend**: Rich SPA with real-time order tracking, GraphQL subscriptions
- **Centralized Logging System**: Production-ready centralized application logging system and analytics
- **Payment Domain Service**: Implementation of a payment system that can handle blockchain payments
- **Infra Management**: Services like Rancher, Grafana, and Prometheus
- **LLM OpenAI Server**: Server responsible for exposing an OpenAI spec API with running LLM models
- **AI Cognitive Service**: Domain service responsible for using LLM models in some cognitive functions, e.g., generate artistic and opinionated product suggestions based on the weather     

### Suggestion of AI/ML Integrations
- **Demand Forecasting**: ML-based inventory prediction
- **Recommendation Engine**: Personalized product suggestions
- **Fraud Detection**: Anomaly detection for payment patterns
- **Chatbot Support**: AI-powered customer service

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

This project is part of a professional portfolio and is available for review by technical and hiring teams.

---

## 👤 Author

**Raphael Pizzaia**

Solution Architect & Full-Stack Developer

- Portfolio: [Link to portfolio]
- LinkedIn: [Link to LinkedIn]
- AI Workspace: [zzaia-agentic-workspace](https://github.com/zzaia/zzaia-agentic-workspace)

---

## 🙏 Acknowledgments

- **Trio Coffee Shop Challenge**: Original challenge concept
- **Claude AI**: AI-assisted development and documentation
- **Microsoft**: .NET platform and Aspire framework
- **Dapr Community**: Service mesh patterns and guidance
