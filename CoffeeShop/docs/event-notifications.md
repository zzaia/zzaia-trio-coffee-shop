# Event Notification System

## Overview

Coffee Shop system uses Kafka for event-driven architecture via Dapr pub/sub. Events enable eventual consistency across services and real-time notifications.

**Infrastructure**: Kafka + Dapr Pub/Sub Component

---

## Event Catalog

### User Events (Published by Identity Service)

#### 1. user.created

**Description**: Published when a new user registers in the system.

**Publisher**: CoffeeShop.Identity
**Subscribers**: CoffeeShop.Order

**Payload Schema**:
```json
{
  "event_id": "uuid",
  "event_type": "user.created",
  "timestamp": "2025-01-15T10:30:00Z",
  "payload": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "john_doe",
    "email": "john.doe@example.com",
    "role": "customer",
    "created_at": "2025-01-15T10:30:00Z"
  }
}
```

**Field Definitions**:
- `event_id` (UUID, required): Unique event identifier for idempotency
- `event_type` (string, required): Event type identifier
- `timestamp` (ISO8601, required): Event publication timestamp
- `payload.id` (UUID, required): User unique identifier
- `payload.username` (string, required): User display name
- `payload.email` (string, required): User email address
- `payload.role` (string, required): "customer" or "manager"
- `payload.created_at` (ISO8601, required): User creation timestamp

**Subscriber Behavior** (Order Service):
- Insert new user record into local Users table
- Enable order placement for this user
- Idempotency: Check if user already exists, skip if duplicate event

---

#### 2. user.updated

**Description**: Published when user profile is updated (username, email, or role changed).

**Publisher**: CoffeeShop.Identity
**Subscribers**: CoffeeShop.Order

**Payload Schema**:
```json
{
  "event_id": "uuid",
  "event_type": "user.updated",
  "timestamp": "2025-01-15T11:45:00Z",
  "payload": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "john_doe_updated",
    "email": "john.updated@example.com",
    "role": "manager",
    "updated_at": "2025-01-15T11:45:00Z"
  }
}
```

**Field Definitions**:
- `event_id` (UUID, required): Unique event identifier
- `event_type` (string, required): Event type identifier
- `timestamp` (ISO8601, required): Event publication timestamp
- `payload.id` (UUID, required): User unique identifier
- `payload.username` (string, required): Updated username
- `payload.email` (string, required): Updated email
- `payload.role` (string, required): Updated role ("customer" or "manager")
- `payload.updated_at` (ISO8601, required): Update timestamp

**Subscriber Behavior** (Order Service):
- Update existing user record in local Users table
- Role change affects future authorization decisions
- Idempotency: Apply update based on `updated_at` timestamp (ignore older events)

---

#### 3. user.deleted

**Description**: Published when a user account is deleted or deactivated.

**Publisher**: CoffeeShop.Identity
**Subscribers**: CoffeeShop.Order

**Payload Schema**:
```json
{
  "event_id": "uuid",
  "event_type": "user.deleted",
  "timestamp": "2025-01-15T12:00:00Z",
  "payload": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "deleted_at": "2025-01-15T12:00:00Z"
  }
}
```

**Field Definitions**:
- `event_id` (UUID, required): Unique event identifier
- `event_type` (string, required): Event type identifier
- `timestamp` (ISO8601, required): Event publication timestamp
- `payload.id` (UUID, required): User unique identifier
- `payload.deleted_at` (ISO8601, required): Deletion timestamp

**Subscriber Behavior** (Order Service):
- Soft delete user record (set `is_active = false`) - preserve order history
- User can no longer place new orders
- Historical orders remain visible to managers
- Idempotency: Check if user already deleted, skip if duplicate event

---

### Order Events (Published by Order Service)

#### 4. order.created

**Description**: Published when a new order is successfully created after payment confirmation.

**Publisher**: CoffeeShop.Order
**Subscribers**: CoffeeShop.BFF (for real-time GraphQL subscriptions)

**Payload Schema**:
```json
{
  "event_id": "uuid",
  "event_type": "order.created",
  "timestamp": "2025-01-15T13:00:00Z",
  "payload": {
    "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
    "user_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "status": "Waiting",
    "total_amount": 12.50,
    "items": [
      {
        "product_name": "Latte",
        "variation_name": "Pumpkin Spice",
        "quantity": 2,
        "unit_price": 4.50
      },
      {
        "product_name": "Donuts",
        "variation_name": "Boston Cream",
        "quantity": 1,
        "unit_price": 3.50
      }
    ],
    "created_at": "2025-01-15T13:00:00Z"
  }
}
```

**Field Definitions**:
- `event_id` (UUID, required): Unique event identifier
- `event_type` (string, required): Event type identifier
- `timestamp` (ISO8601, required): Event publication timestamp
- `payload.id` (UUID, required): Order unique identifier
- `payload.user_id` (UUID, required): Customer who placed the order
- `payload.status` (string, required): Initial status "Waiting"
- `payload.total_amount` (decimal, required): Total order price
- `payload.items` (array, required): Order line items
- `payload.items[].product_name` (string, required): Product name
- `payload.items[].variation_name` (string, required): Variation name
- `payload.items[].quantity` (int, required): Item quantity
- `payload.items[].unit_price` (decimal, required): Price per unit
- `payload.created_at` (ISO8601, required): Order creation timestamp

**Subscriber Behavior** (BFF):
- Trigger GraphQL subscription notification to connected frontend clients
- Cache invalidation for order queries
- Real-time UI update for customers and managers

---

#### 5. order.status.changed

**Description**: Published when an order status transitions (Waiting → Preparation → Ready → Delivered).

**Publisher**: CoffeeShop.Order
**Subscribers**: CoffeeShop.BFF (for real-time GraphQL subscriptions)

**Payload Schema**:
```json
{
  "event_id": "uuid",
  "event_type": "order.status.changed",
  "timestamp": "2025-01-15T13:15:00Z",
  "payload": {
    "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
    "user_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "previous_status": "Waiting",
    "new_status": "Preparation",
    "updated_by": "manager_user_id",
    "updated_at": "2025-01-15T13:15:00Z"
  }
}
```

**Field Definitions**:
- `event_id` (UUID, required): Unique event identifier
- `event_type` (string, required): Event type identifier
- `timestamp` (ISO8601, required): Event publication timestamp
- `payload.id` (UUID, required): Order unique identifier
- `payload.user_id` (UUID, required): Customer who owns the order
- `payload.previous_status` (string, required): Previous status
- `payload.new_status` (string, required): New status (Preparation/Ready/Delivered)
- `payload.updated_by` (UUID, required): Manager who updated the status
- `payload.updated_at` (ISO8601, required): Status update timestamp

**Subscriber Behavior** (BFF):
- Trigger GraphQL subscription notification to connected frontend clients
- Notify customer of order progress via real-time WebSocket
- Update manager's order dashboard in real-time
- Cache invalidation for order status queries

---

## Kafka Topic Configuration

### Topic Naming Convention
`{environment}.{service}.{entity}.{event}`

**Examples**:
- `production.identity.user.created`
- `production.identity.user.updated`
- `production.identity.user.deleted`
- `production.order.order.created`
- `production.order.order.status_changed`

### Topic Configuration
- **Partitions**: 3 (for parallelism)
- **Replication Factor**: 3 (for high availability)
- **Retention**: 7 days
- **Cleanup Policy**: Delete (after retention period)

---

## Dapr Pub/Sub Component Configuration

### Identity Service (Publisher)
```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: pubsub
spec:
  type: pubsub.kafka
  version: v1
  metadata:
    - name: brokers
      value: "localhost:9092"
    - name: authType
      value: "none"
    - name: consumerGroup
      value: "identity-service"
```

### Order Service (Publisher & Subscriber)
```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: pubsub
spec:
  type: pubsub.kafka
  version: v1
  metadata:
    - name: brokers
      value: "localhost:9092"
    - name: authType
      value: "none"
    - name: consumerGroup
      value: "order-service"
```

### BFF Service (Subscriber)
```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: pubsub
spec:
  type: pubsub.kafka
  version: v1
  metadata:
    - name: brokers
      value: "localhost:9092"
    - name: authType
      value: "none"
    - name: consumerGroup
      value: "bff-service"
```

---

## Idempotency Strategy

All event handlers must be idempotent to handle duplicate event deliveries.

### Implementation Patterns

**user.created**:
- Check if user with `id` already exists
- If exists, skip insert
- Log duplicate event for monitoring

**user.updated**:
- Check `updated_at` timestamp
- Only apply if newer than existing record
- Ignore older or duplicate events

**user.deleted**:
- Check if user already soft-deleted
- Skip if already deleted
- Log duplicate event

**order.created**:
- Frontend subscription uses `event_id` for deduplication
- Cache stores `event_id` (TTL: 1 hour)

**order.status.changed**:
- Frontend subscription uses `event_id` for deduplication
- Cache stores `event_id` (TTL: 1 hour)

---

## Error Handling

### Retry Strategy
- Failed event processing triggers retry (max 3 attempts)
- Exponential backoff: 1s, 2s, 4s
- After 3 failures, event moves to dead letter queue (DLQ)

### Dead Letter Queue
- Separate Kafka topic: `{environment}.dlq.{service}`
- Manual review required for DLQ events
- Alert sent to operations team

---

## Monitoring & Observability

### Metrics
- `events_published_total` (counter): Total events published by service
- `events_consumed_total` (counter): Total events consumed by service
- `event_processing_duration_seconds` (histogram): Event handler execution time
- `event_processing_errors_total` (counter): Failed event processing attempts

### Logging
- Log all published events (event_id, event_type, timestamp)
- Log all consumed events with processing status
- Log idempotency skips
- Log DLQ events with failure reason

---

## Testing Strategy

### Unit Tests
- Event serialization/deserialization
- Idempotency logic
- Event validation

### Integration Tests
- End-to-end event flow (publish → consume)
- Verify subscriber behavior
- Test retry logic
- Test DLQ handling

### Load Tests
- 1000 events/second throughput
- Verify no message loss
- Verify ordering within partition
