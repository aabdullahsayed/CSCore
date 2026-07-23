# 10. Introduction to Microservices (Where to Go Next)

## Monolith vs Microservices

```
Monolith:                          Microservices:
┌─────────────────────┐            ┌──────────┐  ┌──────────┐  ┌──────────┐
│   Single API/App     │            │  Books   │  │  Orders  │  │  Users   │
│  (Books, Orders,      │   vs.     │  Service │  │ Service  │  │ Service  │
│   Users, all-in-one)  │            └────┬─────┘  └────┬─────┘  └────┬─────┘
└─────────────────────┘                   └────────────┬┴─────────────┘
                                                   API Gateway
```

Everything you've learned so far builds **one well-structured service**. That's not wasted effort — most companies are well-served by a well-organized monolith (sometimes called a "modular monolith") for a long time before microservices become necessary.

## When microservices make sense (and when they don't)

**Consider microservices when:**
- Different parts of the system need to scale independently (e.g., image processing vs. simple CRUD).
- Multiple teams need to deploy independently without blocking each other.
- Different parts have very different technology needs.

**Avoid microservices when:**
- You're a small team or early-stage product — the operational overhead (deployment, monitoring, network reliability, distributed debugging) usually outweighs the benefits.
- You haven't yet identified clear service boundaries — splitting too early often means splitting in the wrong places.

> Common advice: "Start with a monolith, split into services only when you feel real pain from not doing so."

## Core concepts you'd learn next

### 1. Service-to-service communication
- **Synchronous**: HTTP/REST or gRPC calls between services.
- **Asynchronous**: message queues/event buses (RabbitMQ, Kafka, Azure Service Bus) — services publish events, others react, without direct coupling.

### 2. HttpClientFactory for calling other services

```csharp
builder.Services.AddHttpClient("OrdersService", client =>
{
    client.BaseAddress = new Uri("https://orders-service.internal/");
    client.Timeout = TimeSpan.FromSeconds(10);
});
```

```csharp
public class OrdersClient
{
    private readonly HttpClient _client;
    public OrdersClient(IHttpClientFactory factory) => _client = factory.CreateClient("OrdersService");

    public async Task<OrderDto?> GetOrderAsync(int id) =>
        await _client.GetFromJsonAsync<OrderDto>($"api/orders/{id}");
}
```

### 3. API Gateway
A single entry point that routes client requests to the right backend service, often also handling auth, rate limiting, and request aggregation. Common options: **YARP** (.NET's own reverse proxy library), Ocelot, or cloud-managed gateways (Azure API Management, AWS API Gateway).

### 4. Resilience — retries, circuit breakers, timeouts

```bash
dotnet add package Microsoft.Extensions.Http.Resilience
```

```csharp
builder.Services.AddHttpClient("OrdersService")
    .AddStandardResilienceHandler();   // built-in retry + circuit breaker + timeout policies
```

Distributed systems fail partially and unpredictably — resilience patterns (retry with backoff, circuit breakers to stop hammering a failing service, timeouts) are essential, not optional.

### 5. Distributed tracing
Once a request spans multiple services, a single trace ID (see `03-Global-Error-Handling-Logging.md`) needs to flow through every service call so you can reconstruct the full request path. **OpenTelemetry** is the current industry-standard tool for this.

### 6. Data ownership
Each microservice should own its own database — no service reaches directly into another's tables. Cross-service data needs are handled via API calls or eventual consistency through events, not shared databases.

## Suggested learning path from here

1. Build 2 small services (e.g., `Books` and `Orders`) that call each other via HTTP.
2. Add an API Gateway (YARP) in front of them.
3. Introduce a message broker (RabbitMQ) for one async workflow (e.g., "Order placed" event → `Books` service decrements stock).
4. Containerize both with Docker Compose (you already know Docker from file 08).
5. Explore **Kubernetes** once you're comfortable orchestrating multiple containers by hand.

## Closing note

You now have a complete path: routing and controllers → data access → validation → security → resilience → deployment. The single biggest skill left to build is **judgment** — knowing which of these tools a given project actually needs. Not every API needs Redis, JWT refresh tokens, or microservices on day one. Build the simplest thing that solves the real problem, and reach for these tools as real needs surface.

---
**Congratulations — you've completed the course.**
Return to `../00-README.md` for the full roadmap, or check `../04-Resources/01-Cheatsheet-And-Further-Reading.md` for a quick reference.
