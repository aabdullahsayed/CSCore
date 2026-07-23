# 3. Microservices Basics with ASP.NET Core

## What are microservices?

An architectural style where an application is split into small, independently deployable services, each owning its own data and business capability, communicating over the network (HTTP/gRPC/messaging) rather than in-process calls.

```
┌────────────┐   ┌────────────┐   ┌────────────┐
│ Books API  │   │ Orders API │   │ Users API  │
│ (own DB)   │   │ (own DB)   │   │ (own DB)   │
└─────┬──────┘   └─────┬──────┘   └─────┬──────┘
      │                │                │
      └────────────────┼────────────────┘
                        │
                 ┌──────▼──────┐
                 │  API Gateway │
                 └──────┬──────┘
                        │
                     Clients
```

## When (not) to use microservices

**Good fit**: large teams needing independent deployability, different scaling needs per service, polyglot tech requirements, well-understood domain boundaries.

**Poor fit**: small teams, early-stage products with an evolving domain, or when the main goal is just "it sounds modern." A **modular monolith** (one deployable app, cleanly separated internal modules) captures most of the maintainability benefits with far less operational overhead — many teams should start there and split into microservices only once real scaling/team pain justifies it.

## Service-to-service communication

**Synchronous (HTTP) via `HttpClientFactory`:**

```csharp
builder.Services.AddHttpClient("OrdersApi", client =>
{
    client.BaseAddress = new Uri("https://orders-api.internal/");
    client.Timeout = TimeSpan.FromSeconds(10);
});
```

```csharp
public class OrdersClient
{
    private readonly HttpClient _client;
    public OrdersClient(IHttpClientFactory factory) => _client = factory.CreateClient("OrdersApi");

    public async Task<OrderDto?> GetOrderAsync(int id) =>
        await _client.GetFromJsonAsync<OrderDto>($"api/orders/{id}");
}
```

`IHttpClientFactory` manages connection pooling and avoids the socket-exhaustion problems of manually `new HttpClient()`-ing per request.

**Asynchronous (messaging) with RabbitMQ / Azure Service Bus:**

Use a message broker when services should be decoupled in time — the publisher doesn't wait for a response, and consumers process events independently. Common with libraries like **MassTransit**:

```csharp
public record BookCreatedEvent(int BookId, string Title);

// Publisher (Books API)
await _publishEndpoint.Publish(new BookCreatedEvent(book.Id, book.Title));

// Consumer (Notifications service)
public class BookCreatedConsumer : IConsumer<BookCreatedEvent>
{
    public async Task Consume(ConsumeContext<BookCreatedEvent> context)
    {
        // send an email, update a search index, etc.
    }
}
```

This is the foundation of **event-driven architecture** — services react to things that happened rather than being directly called.

## Resilience: retries, circuit breakers, timeouts

Network calls fail. Use **Polly** (built into `Microsoft.Extensions.Http.Resilience` since .NET 8) to handle transient failures gracefully:

```bash
dotnet add package Microsoft.Extensions.Http.Resilience
```

```csharp
builder.Services.AddHttpClient("OrdersApi")
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.CircuitBreaker.FailureRatio = 0.5;
    });
```

This adds automatic retries with exponential backoff, a circuit breaker (stop calling a service that's clearly down), and a timeout — all with a couple of lines.

## API Gateway pattern

A single entry point that routes requests to the right backend service, and can also handle cross-cutting concerns (auth, rate limiting, aggregation). **YARP** (Yet Another Reverse Proxy) is Microsoft's toolkit for building one in .NET:

```bash
dotnet new web -n Gateway
dotnet add package Yarp.ReverseProxy
```

```json
{
  "ReverseProxy": {
    "Routes": {
      "booksRoute": { "ClusterId": "booksCluster", "Match": { "Path": "/api/books/{**catch-all}" } }
    },
    "Clusters": {
      "booksCluster": { "Destinations": { "d1": { "Address": "https://books-api.internal/" } } }
    }
  }
}
```

## Data consistency across services

Since each service owns its own database, you can't rely on cross-service database transactions. Two common patterns:

- **Saga pattern** — a sequence of local transactions, each publishing an event that triggers the next step; compensating actions undo prior steps on failure.
- **Eventual consistency** — accept that data across services converges over time (via events) rather than instantaneously.

## Practice task

Split the Books API into two services — `Books.Api` and `Notifications.Api` — where creating a book publishes a `BookCreatedEvent` that the Notifications service consumes and logs.

**Next → `04-docker-and-deployment.md`**
