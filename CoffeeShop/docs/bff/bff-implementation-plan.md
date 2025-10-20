# CoffeeShop.BFF Implementation Plan

## Overview

Sequential implementation for BFF service as API Gateway with GraphQL, Dapr service invocation, Redis caching, and Kafka event subscriptions for real-time frontend notifications.

**Duration**: 27-35 days | **Tech**: .NET 9, HotChocolate GraphQL, MediatR, FluentValidation, Mapster, Dapr, Redis, Kafka

---

## Phase 1: Domain Layer (2 days)

- [ ] Create domain DTOs (`MenuDto`, `ProductDto`, `ProductVariationDto`, `OrderDto`, `OrderItemDto`, `UserDto`)
- [ ] Implement value objects (`OrderStatus` enum)
- [ ] Create domain exceptions (`ServiceUnavailableException`, `UnauthorizedException`, `InvalidRequestException`)
- [ ] Write unit tests for domain logic

---

## Phase 2: Service Interfaces (2 days)

- [ ] Define service interfaces (`IIdentityService`, `IOrderService`)
- [ ] Create command models (`CreateOrderCommand`, `UpdateOrderStatusCommand`, `LoginCommand`, `RegisterUserCommand`)
- [ ] Create query models (`GetMenuQuery`, `GetOrderByIdQuery`, `GetAllOrdersQuery`, `GetUserByIdQuery`)
- [ ] Configure Mapster mapping profiles (service DTOs → GraphQL types)

---

## Phase 3: Dapr Service Invocation (3-4 days)

- [ ] Install and configure Dapr SDK (AspNetCore, Client)
- [ ] Implement `IdentityService.cs` with Dapr invocation (auth, registration, token validation)
- [ ] Implement `OrderService.cs` with Dapr invocation (menu, orders, status updates)
- [ ] Configure Polly policies (circuit breaker, retry, timeout 30s)
- [ ] Create `DaprServiceInvoker.cs` wrapper with error handling
- [ ] Test service invocation with mocks

---

## Phase 4: Redis Caching (2-3 days)

- [ ] Configure StackExchange.Redis client with Dapr secret store
- [ ] Implement `RedisCacheService.cs` with cache key generation (request checksum)
- [ ] Configure cache TTLs (menu: 15 min, orders: 5 min, users: 5 min)
- [ ] Implement cache invalidation (order status change, user profile update)
- [ ] Test Redis integration

---

## Phase 5: Commands and Queries (3-4 days)

- [ ] Implement command handlers (CreateOrder, UpdateOrderStatus, Login, RegisterUser)
- [ ] Implement query handlers with caching (GetMenu, GetOrderById, GetAllOrders, GetUserById)
- [ ] Configure MediatR pipeline behaviors (ValidationBehavior, LoggingBehavior, CachingBehavior, CircuitBreakerBehavior)
- [ ] Implement FluentValidation validators for all commands
- [ ] Write unit tests for handlers

---

## Phase 6: Event Subscriptions (2-3 days)

- [ ] Configure Dapr pub/sub component for Kafka
- [ ] Subscribe to `order.created` and `order.status.changed` from Order service
- [ ] Implement `OrderStatusChangedEventHandler.cs`
- [ ] Trigger GraphQL subscriptions on events
- [ ] Implement cache invalidation on events
- [ ] Test event subscription

---

## Phase 7: GraphQL Schema (3-4 days)

- [ ] Install and configure HotChocolate GraphQL
- [ ] Setup GraphQL endpoint (`/graphql`) and Banana Cake Pop UI
- [ ] Implement GraphQL queries (`MenuQueries`, `OrderQueries`, `UserQueries`)
- [ ] Implement GraphQL mutations (`OrderMutations`, `AuthMutations`)
- [ ] Implement GraphQL subscriptions (`OrderSubscriptions` for real-time status changes)
- [ ] Configure GraphQL authorization (`[Authorize]`, role-based field authorization)

---

## Phase 8: Authentication & Authorization (2-3 days)

- [ ] Configure JWT authentication middleware
- [ ] Configure Identity service as authority
- [ ] Configure authorization policies (Customer, Manager)
- [ ] Apply policies to GraphQL fields and resolvers
- [ ] Implement token forwarding to Dapr service invocations
- [ ] Implement correlation ID propagation

---

## Phase 9: Cross-Cutting Concerns (2-3 days)

- [ ] Implement exception handling middleware (Problem Details RFC 7807, GraphQL error formatting)
- [ ] Configure Serilog with correlation IDs (log GraphQL operations)
- [ ] Implement Dapr secret store integration
- [ ] Configure health checks (Redis, Dapr sidecar, external services)
- [ ] Implement rate limiting middleware (per-user, per-IP)

---

## Phase 10: Testing (4-5 days)

- [ ] Unit tests for application layer (handlers with mocked service clients)
- [ ] Integration tests for GraphQL endpoints (queries, mutations, mocked backends)
- [ ] Integration tests for authentication and authorization
- [ ] Integration tests for Dapr service invocation (Identity, Order)
- [ ] Integration tests for circuit breaker and retry behavior
- [ ] Integration tests for event subscriptions and GraphQL notifications
- [ ] Integration tests for Redis caching
- [ ] Load testing (GraphQL queries, concurrent subscriptions, cache performance)

---

## Phase 11: Deployment (2-3 days)

- [ ] Create multi-stage Dockerfile with security scanning
- [ ] Configure .NET Aspire integration
- [ ] Generate Kubernetes manifests with Aspire8
- [ ] Setup CI/CD pipeline
- [ ] Validate deployment

---

## Success Criteria

- ✅ GraphQL endpoint functional (queries, mutations, subscriptions)
- ✅ Dapr service invocation to Identity and Order services
- ✅ Redis caching with invalidation
- ✅ Kafka event subscriptions functional
- ✅ Real-time GraphQL subscriptions for order status changes
- ✅ JWT authentication and role-based authorization
- ✅ Circuit breaker and retry mechanisms operational
- ✅ Rate limiting enforced
- ✅ Test coverage >80%
- ✅ Deployment-ready with Kubernetes manifests
