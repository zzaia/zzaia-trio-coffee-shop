# CoffeeShop.Identity Implementation Plan

## Overview

Sequential implementation for Identity service with OpenIddict OAuth2/OIDC supporting user authentication (ROPC) and service-to-service authentication (client credentials), with Kafka event publishing via Dapr.

**Duration**: 29-38 days | **Tech**: .NET 9, OpenIddict, EF Core, PostgreSQL 15, Redis 7, MediatR, FluentValidation, Mapster, Dapr, Kafka, BCrypt

---

## Phase 1: Domain Layer (2-3 days)

- [ ] Create domain entities (`User.cs`, `RefreshToken.cs`, `Application.cs`, `ApplicationToken.cs`)
- [ ] Implement value objects (`Email.cs`, `Password.cs`)
- [ ] Define `UserRole` enum (Customer, Manager)
- [ ] Create domain events (`UserCreatedEvent`, `UserUpdatedEvent`, `UserDeletedEvent`)
- [ ] Implement user aggregate business rules (email uniqueness, password complexity)
- [ ] Write unit tests for domain logic

---

## Phase 2: Database Setup (2-3 days)

- [ ] Configure PostgreSQL connection with Dapr secret store
- [ ] Create EF Core migrations (Users, RefreshTokens, Applications, ApplicationTokens, OpenIddict tables)
- [ ] Configure indexes (unique on email/username/client_id, indexes on tokens)
- [ ] Configure ServiceDefaults for auto-migrations
- [ ] Test database connectivity

---

## Phase 3: Redis Caching (1-2 days)

- [ ] Configure StackExchange.Redis client with Dapr secret store
- [ ] Implement `RedisCacheService.cs` (token caching, user profile caching)
- [ ] Configure TTL (tokens: sliding expiration, users: 5 minutes)
- [ ] Implement cache invalidation logic
- [ ] Test Redis integration

---

## Phase 4: OpenIddict Configuration (3-4 days)

- [ ] Install OpenIddict packages (AspNetCore, EntityFrameworkCore)
- [ ] Configure OpenIddict core (EF Core stores, endpoint URIs)
- [ ] Configure OpenIddict server (Password, Refresh Token, Client Credentials flows)
- [ ] Register scopes (openid, profile, email, roles)
- [ ] Configure token lifetimes (access: 15 min, refresh: 30 days users / 5 min apps)
- [ ] Configure signing/encryption keys (ephemeral for dev, persistent for prod)
- [ ] Configure OpenIddict validation

---

## Phase 5: User Authentication Commands (3-4 days)

- [ ] Implement `RegisterUserCommand` with FluentValidation and BCrypt hashing (cost 12)
- [ ] Implement `LoginCommand` with credential validation and OpenIddict token generation
- [ ] Implement `RefreshTokenCommand` with token rotation and revocation
- [ ] Configure MediatR pipeline behaviors (ValidationBehavior, LoggingBehavior)
- [ ] Write unit tests for command handlers

---

## Phase 6: Application Authentication Commands (2-3 days)

- [ ] Implement `RegisterApplicationCommand` (generate client_id/client_secret, BCrypt hashing)
- [ ] Implement `AuthenticateApplicationCommand` (validate credentials, generate token)
- [ ] Create repository interfaces (`IUserRepository`, `IApplicationRepository`, `IRefreshTokenRepository`)
- [ ] Write unit tests for application commands

---

## Phase 7: Queries (2-3 days)

- [ ] Implement `GetUserByIdQuery` with Redis caching
- [ ] Implement `ValidateTokenQuery` using OpenIddict validation
- [ ] Implement `ValidateApplicationTokenQuery` with Redis check
- [ ] Configure Mapster mapping profiles (entities → DTOs)
- [ ] Write unit tests for query handlers

---

## Phase 8: Event Publishing (2-3 days)

- [ ] Configure Dapr pub/sub component for Kafka
- [ ] Implement `KafkaEventPublisher.cs` using Dapr SDK
- [ ] Publish `user.created`, `user.updated`, `user.deleted` events
- [ ] Integrate event publishing into command handlers
- [ ] Test event publishing

---

## Phase 9: API Endpoints (2-3 days)

- [ ] Configure OpenIddict token endpoint (`POST /connect/token`)
- [ ] Configure OpenIddict userinfo endpoint (`GET /connect/userinfo`)
- [ ] Implement `AuthController.cs` for registration
- [ ] Implement `UserController.cs` for user management
- [ ] Configure endpoint routing and CORS
- [ ] Add Swagger/OpenAPI documentation

---

## Phase 10: Authentication & Authorization (2-3 days)

- [ ] Configure JWT authentication middleware
- [ ] Configure OpenIddict as token issuer
- [ ] Configure authorization policies (Customer, Manager)
- [ ] Apply policies to endpoints
- [ ] Implement rate limiting (login: 5/15min, registration: 3/hour, refresh: 10/min)

---

## Phase 11: Cross-Cutting Concerns (2-3 days)

- [ ] Implement exception handling middleware (Problem Details RFC 7807)
- [ ] Configure Serilog with correlation IDs
- [ ] Implement Dapr secret store integration
- [ ] Configure health checks (database, Redis, Dapr sidecar)

---

## Phase 12: Testing (4-5 days)

- [ ] Unit tests for domain layer (aggregates, value objects, business rules)
- [ ] Unit tests for application layer (handlers with mocked dependencies)
- [ ] Integration tests for authentication flows (registration, ROPC, refresh, client credentials)
- [ ] Integration tests for token validation (TestContainers)
- [ ] Integration tests for event publishing (mocked Kafka)
- [ ] Integration tests for Redis caching
- [ ] Load testing (login, token validation)

---

## Phase 13: Deployment (2-3 days)

- [ ] Create multi-stage Dockerfile with security scanning
- [ ] Configure .NET Aspire integration
- [ ] Generate Kubernetes manifests with Aspire8
- [ ] Setup CI/CD pipeline
- [ ] Validate deployment

---

## Success Criteria

- ✅ OpenIddict OAuth2/OIDC endpoints functional
- ✅ User registration and login (ROPC) working
- ✅ Refresh token flow operational
- ✅ Client credentials flow for service-to-service auth
- ✅ Kafka event publishing for user lifecycle
- ✅ Redis caching operational
- ✅ BCrypt password hashing (cost 12)
- ✅ Rate limiting enforced
- ✅ Test coverage >80%
- ✅ Deployment-ready with Kubernetes manifests
