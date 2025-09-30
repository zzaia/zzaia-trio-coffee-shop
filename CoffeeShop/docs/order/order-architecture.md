# CoffeeShop.Order Detailed Architecture

## Trio Coffee Shop Challenge Description

The Trio Challenge consists of creating a RESTful coffee shop order management application.

### Getting Started

This application needs to support two types of users: **Managers** and **Customers**.

- **Managers** have full access to all features.
- **Customers** have limited access and cannot perform managerial actions.

To simulate role-based access, requests should include a role header with one of the following values:

```
role: customer
role: manager
```

### Managers

Orders can have one of the four statuses: **Waiting**, **Preparation**, **Ready**, and **Delivered**.

Status transitions must follow this strict sequence:
**Status Flow**: Waiting → Preparation → Ready → Delivered

### Customers

Order and customize their orders with several options from the catalog below.

### Catalog with Pricing

| Product | Base Price | Variation | Price Change |
|---------|-----------|-----------|--------------|
| Latte | $4.00 | Pumpkin Spice | +$0.50 |
| Latte | $4.00 | Vanilla | +$0.30 |
| Latte | $4.00 | Hazelnut | +$0.40 |
| Espresso | $2.50 | Single Shot | +$0.00 |
| Espresso | $2.50 | Double Shot | +$1.00 |
| Macchiato | $4.00 | Caramel | +$0.50 |
| Macchiato | $4.00 | Vanilla | +$0.30 |
| Iced Coffee | $3.50 | Regular | +$0.00 |
| Iced Coffee | $3.50 | Sweetened | +$0.30 |
| Iced Coffee | $3.50 | Extra Ice | +$0.20 |
| Donuts | $2.00 | Glazed | +$0.00 |
| Donuts | $2.00 | Jelly | +$0.30 |
| Donuts | $2.00 | Boston Cream | +$0.50 |

### Required Endpoints

#### 1. GET /menu — View Menu
Returns a complete list of products with:
- Base prices
- All available variations
- Price changes for each variation (as shown in the catalog)

#### 2. POST /orders — Place a New Order
- Accepts a list of products and their variations
- Calculates the total price based on base price + variation price
- **Critical Requirement**: Integrates with the payment service:
  - `POST https://challenge.trio.dev/api/v1/payment`
  - `{"value": TOTAL_AMOUNT}`
  - Displays full payment service response in the terminal
  - Order is only created if the payment is successful
  - All orders are initialized with the status: **Waiting**
  - Returns appropriate error if payment fails

#### 3. GET /orders/{id} — View Order Details
Returns full information for a specific order:
- All ordered items and their variations
- Individual item pricing
- Total order price
- Current order status
- Order creation timestamp

#### 4. GET /orders — List All Orders
- **Manager Only**: Enforces role-based access control
- Returns a list of all current orders that must be executed
- Each order includes:
  - Order ID
  - Customer information
  - Current status
  - Total price
  - Order creation timestamp
- Orders are sorted by creation time (oldest first)
- Useful for managers to track all pending and in-progress orders

#### 5. PATCH /orders/{id}/status — Update Order Status
- **Manager Only**: Enforces role-based access control
- Status transitions must follow strictly:
  - **Status Flow**: Waiting → Preparation → Ready → Delivered
- **Critical Requirement**: Integrates with the notification service:
  - `POST https://challenge.trio.dev/api/v1/notification`
  - `{"status": "{ORDER_STATUS}"}`
  - Displays full notification response in the terminal

## C4 Context Diagram - Order Application

```mermaid
C4Context
    title System Context diagram for CoffeeShop Order Application

    Person(customer, "Customer", "Coffee shop customer placing orders")
    Person(manager, "Manager", "Coffee shop manager managing orders")

    System(orderSystem, "Order Service", "Manages coffee shop orders, pricing, and status transitions")

    System_Ext(identitySystem, "Identity Service", "Handles authentication and authorization using client credentials")
    System_Ext(paymentService, "Payment Service", "External payment processing service")
    System_Ext(notificationService, "Notification Service", "External notification service for order status updates")

    SystemDb(orderDb, "PostgreSQL Database", "Stores order data, items, and status")
    System_Ext(daprSidecar, "Dapr Sidecar", "Service mesh for inter-service communication and secret management")

    Rel(customer, orderSystem, "Places orders, views order details", "HTTPS/REST")
    Rel(manager, orderSystem, "Updates order status, views all orders", "HTTPS/REST")

    Rel(orderSystem, identitySystem, "Authenticates requests, validates roles", "Dapr Service Invocation")
    Rel(orderSystem, paymentService, "Processes payments synchronously", "HTTPS/REST")
    Rel(orderSystem, notificationService, "Sends status change notifications", "HTTPS/REST")
    Rel(orderSystem, orderDb, "Reads from and writes to", "EF Core")
    Rel(orderSystem, daprSidecar, "Uses for service discovery, secrets", "Dapr Client SDK")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

## C4 Container Diagram - Order Application

```mermaid
C4Container
    title Container diagram for CoffeeShop Order Application

    Person(customer, "Customer", "Coffee shop customer")
    Person(manager, "Manager", "Coffee shop manager")

    Container_Boundary(orderBoundary, "Order Service") {
        Container(api, "REST API", "ASP.NET Core, FastAPI", "Exposes order management endpoints")
        Container(application, "Application Layer", "MediatR, FluentValidation", "Handles CQRS commands and queries")
        Container(domain, "Domain Layer", "C# Domain Models", "Core business logic and rules")
        Container(infrastructure, "Infrastructure Layer", "EF Core, HttpClient", "Data access and external integrations")
    }

    ContainerDb(database, "Order Database", "PostgreSQL", "Stores orders and items")

    System_Ext(identityService, "Identity Service", "Authentication provider")
    System_Ext(paymentAPI, "Payment API", "Payment processing")
    System_Ext(notificationAPI, "Notification API", "Status notifications")
    Container_Ext(dapr, "Dapr", "Service mesh runtime")

    Rel(customer, api, "Uses", "HTTPS/REST")
    Rel(manager, api, "Uses", "HTTPS/REST")

    Rel(api, application, "Sends commands/queries to", "MediatR")
    Rel(application, domain, "Uses domain logic from")
    Rel(application, infrastructure, "Calls repositories and services")

    Rel(infrastructure, database, "Reads/Writes", "EF Core/SQL")
    Rel(infrastructure, paymentAPI, "Processes payments", "HTTP + Polly")
    Rel(infrastructure, notificationAPI, "Sends notifications", "HTTP + Polly")
    Rel(infrastructure, identityService, "Validates auth", "Dapr Client")

    Rel(api, dapr, "Uses for secrets and discovery", "Dapr SDK")

    UpdateLayoutConfig($c4ShapeInRow="2", $c4BoundaryInRow="1")
```

## Order Creation Sequence Diagram

```mermaid
sequenceDiagram
    participant Customer as Customer App
    participant Manager as Manager App
    participant BFF as Backend for Frontend
    participant OrderService as Order Service
    participant PaymentService as Payment Service
    participant NotificationService as Notification Service
    participant Database as PostgreSQL

    Note over Customer,Database: Customer Order Creation Flow
    Customer->>BFF: POST /orders with order details
    BFF->>OrderService: Validate and process order
    OrderService->>OrderService: Validate order items
    OrderService->>OrderService: Calculate total price
    OrderService->>PaymentService: Process payment (synchronous)
    PaymentService-->>OrderService: Payment confirmation (success)
    OrderService->>Database: Save order with status "Waiting"
    OrderService-->>BFF: Order created response
    BFF-->>Customer: Order confirmation

    Note over Manager,Database: Manager Status Update Flow (3 Sequential Requests)
    Manager->>BFF: PATCH /orders/{id}/status (Waiting → Preparation)
    BFF->>OrderService: Update status to Preparation
    OrderService->>Database: Update order status
    OrderService->>NotificationService: Notify status change
    NotificationService-->>OrderService: Notification confirmed
    OrderService-->>BFF: Status updated
    BFF-->>Manager: Confirmation

    Manager->>BFF: PATCH /orders/{id}/status (Preparation → Ready)
    BFF->>OrderService: Update status to Ready
    OrderService->>Database: Update order status
    OrderService->>NotificationService: Notify status change
    NotificationService-->>OrderService: Notification confirmed
    OrderService-->>BFF: Status updated
    BFF-->>Manager: Confirmation

    Manager->>BFF: PATCH /orders/{id}/status (Ready → Delivered)
    BFF->>OrderService: Update status to Delivered
    OrderService->>Database: Update order status
    OrderService->>NotificationService: Notify status change
    NotificationService-->>OrderService: Notification confirmed
    OrderService-->>BFF: Status updated
    BFF-->>Manager: Confirmation
```

## Clean Architecture Layer Structure

```
CoffeeShop.Order/
├── Domain/
│   ├── Entities/
│   │   ├── Order.cs
│   │   └── OrderItem.cs
│   ├── ValueObjects/
│   │   ├── Money.cs
│   │   ├── Quantity.cs
│   │   └── ProductSnapshot.cs
│   ├── Aggregates/
│   │   └── OrderAggregate.cs
│   ├── DomainEvents/
│   │   ├── OrderCreatedEvent.cs
│   │   └── OrderStatusChangedEvent.cs
│   └── Enums/
│       └── OrderStatus.cs
├── Application/
│   ├── Commands/
│   │   ├── CreateOrderCommand.cs
│   │   └── UpdateOrderStatusCommand.cs
│   ├── Queries/
│   │   ├── GetMenuQuery.cs
│   │   ├── GetOrderByIdQuery.cs
│   │   └── GetAllOrdersQuery.cs
│   ├── DTOs/
│   │   ├── OrderDto.cs
│   │   └── OrderItemDto.cs
│   └── Interfaces/
│       └── IOrderRepository.cs
├── Infrastructure/
│   ├── Persistence/
│   │   └── OrderRepository.cs
│   ├── ExternalServices/
│   │   ├── PaymentService.cs
│   │   └── NotificationService.cs
│   └── Repositories/
│       └── PostgreSqlOrderRepository.cs
└── Presentation/
    └── Controllers/
        └── OrderController.cs
```

## Order Status Transition Workflow

```mermaid
stateDiagram-v2
    [*] --> Waiting: Create Order
    Waiting --> Preparation: Start Preparation (Manager)
    Preparation --> Ready: Complete Preparation (Manager)
    Ready --> Delivered: Deliver Order (Manager)
    Delivered --> [*]
```

## Key Design Decisions
- Resilient external service communication
- Strict order status transitions

## Pricing Calculation Strategy
- Base product price
- Variation price modifiers
- Validation of pricing rules
- Immutable price calculations

## Exception Handling
- Use of Exception handler middleware to generate problem details responses
- Return problem details with validation errors from fluent validation exceptions

## Event driven design
- Use of CQRS with mediator pattern
- Use of pipeline behavior in mediator for validation and logging

## Logging
- Avoid logging throughout the code
- Use the logging pipeline behavior pattern to log commands, responses and exceptions
- Let the  

## Authentication & Authorization 
- Client credential authentication using the Identity application
- Policies checking for claims, based on the role customer or manager  

## Exposed end-points
- Rest API exposed using the FastAPI

## Mapping
- Classes must be mapped using the Mapster 
- Implement the profile pattern injected in the DI

## Internal service integration
- Use the dapr client to call internal services, like Identity application

## Secret Store
- Use of the dapr client secret-store integration
- Retrieve all secrets during application initialization and add to DI

## Validation Rules
- Use of fluent validation defined in the DI
- Use the validation pipeline behavior to validate commands, and responses
- Prevent invalid order creation
- Enforce business rules
- Validate product availability
- Check customer roles for status changes

## Polly External Services Integration
- Circuit breaker for Payment service
- Locking mechanism to avoid double payment calling for the same order id;
- Retry mechanisms
- Timeout configurations
- Async communication patterns

## Persistency
- Use of PostgreSQL to persist all domain models
- Use Entity Framework
- 

## Recommended Tools
- .NET 9 
- ASP NET Core
- FastAPI
- MediatR
- FluentValidation
- Entity Framework Core
- Mapster
- Dapr
- PostgreSQL
- Docker
- Kubernetes
- Polly