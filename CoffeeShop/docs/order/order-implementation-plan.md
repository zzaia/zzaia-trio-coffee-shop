# CoffeeShop.Order Implementation Plan

## Overview

Sequential implementation for Order service with payment integration and Kafka event publishing via Dapr.

**Duration**: 29-38 days | **Tech**: .NET 9, EF Core, PostgreSQL 15, MediatR, FluentValidation, Mapster, Dapr, Polly, Kafka

---

## Phase 1: Domain Layer (3-4 days)

- [ ] Create domain entities (`Order.cs`, `OrderItem.cs`, `Product.cs`, `ProductVariation.cs`)
- [ ] Implement value objects (`Money.cs`, `Quantity.cs`, `ProductSnapshot.cs`)
- [ ] Define `OrderStatus` enum (Waiting → Preparation → Ready → Delivered)
- [ ] Create domain events (`OrderCreatedEvent`, `OrderStatusChangedEvent`)
- [ ] Implement order aggregate business rules and validations
- [ ] Write unit tests for domain logic

---

## Phase 2: Database Setup (2-3 days)

- [ ] Configure PostgreSQL connection with Dapr secret store
- [ ] Create EF Core migrations (Users, Orders, OrderItems, Products, ProductVariations)
- [ ] Seed product catalog data via migration
- [ ] Configure ServiceDefaults for auto-migrations
- [ ] Test database connectivity and data integrity

---

## Phase 3: Application Commands (3-4 days)

- [ ] Implement `CreateOrderCommand` with FluentValidation and payment integration
- [ ] Implement `UpdateOrderStatusCommand` with status transition validation
- [ ] Configure MediatR pipeline behaviors (ValidationBehavior, LoggingBehavior)
- [ ] Create repository interfaces (`IOrderRepository`, `IProductRepository`)
- [ ] Write unit tests for command handlers

---

## Phase 4: Application Queries (2-3 days)

- [ ] Implement `GetMenuQuery` (products with variations)
- [ ] Implement `GetOrderByIdQuery` with authorization check
- [ ] Implement `GetAllOrdersQuery` (manager only, sorted by creation time)
- [ ] Configure Mapster mapping profiles (entities → DTOs)
- [ ] Write unit tests for query handlers

---

## Phase 5: External Services (3-4 days)

- [ ] Implement `PaymentService.cs` with Polly (circuit breaker, retry, timeout, locking)
- [ ] Implement `ExternalNotificationService.cs` with Polly
- [ ] Create `IdentityServiceClient.cs` using Dapr service invocation
- [ ] Configure client credentials authentication
- [ ] Test external service integration with mocks

---

## Phase 6: Event Publishing (2-3 days)

- [ ] Configure Dapr pub/sub component for Kafka
- [ ] Implement `EventPublisher.cs` using Dapr SDK
- [ ] Publish `order.created` and `order.status.changed` events
- [ ] Subscribe to `user.created`, `user.updated`, `user.deleted` from Identity
- [ ] Update local user cache from events
- [ ] Test event publishing and subscription

---

## Phase 7: API Endpoints (3-4 days)

- [ ] Implement `GET /menu` endpoint with Swagger docs
- [ ] Implement `POST /orders` (payment → create order → publish event)
- [ ] Implement `GET /orders/{id}` with authorization
- [ ] Implement `GET /orders` (manager only)
- [ ] Implement `PATCH /orders/{id}/status` (manager only, notification → event)
- [ ] Configure FastAPI routing

---

## Phase 8: Authentication & Authorization (2-3 days)

- [ ] Configure JWT authentication middleware
- [ ] Setup Identity service as authority
- [ ] Implement client credentials token acquisition
- [ ] Configure authorization policies (Customer, Manager)
- [ ] Apply policies to endpoints

---

## Phase 9: Cross-Cutting Concerns (2-3 days)

- [ ] Implement exception handling middleware (Problem Details RFC 7807)
- [ ] Configure Serilog with correlation IDs
- [ ] Implement Dapr secret store integration
- [ ] Configure health checks (database, external services, Dapr sidecar)

---

## Phase 10: Testing (4-5 days)

- [ ] Unit tests for domain layer (aggregates, value objects, business rules)
- [ ] Unit tests for application layer (handlers with mocked dependencies)
- [ ] Integration tests for API endpoints (TestContainers)
- [ ] Integration tests for payment and notification (mocked services)
- [ ] Integration tests for event publishing (mocked Kafka)
- [ ] Integration tests for user event subscription
- [ ] Load testing (order creation, status updates)

---

## Phase 11: Deployment (2-3 days)

- [ ] Create multi-stage Dockerfile with security scanning
- [ ] Configure .NET Aspire integration
- [ ] Generate Kubernetes manifests with Aspire8
- [ ] Setup CI/CD pipeline
- [ ] Validate deployment

---

## Success Criteria

- ✅ All 5 REST endpoints functional
- ✅ Payment integration with circuit breaker
- ✅ External notification service integration
- ✅ Kafka event publishing operational
- ✅ User synchronization from Identity events
- ✅ Test coverage >80%
- ✅ Role-based authorization enforced
- ✅ Deployment-ready with Kubernetes manifests
