# 6. Caching

## Why cache?

Reduces database load and latency for data that doesn't change on every request (e.g., product catalogs, reference data). Three main layers: in-memory, distributed, and HTTP response caching.

## In-memory caching

```bash
# Microsoft.Extensions.Caching.Memory is included by default
```

```csharp
builder.Services.AddMemoryCache();
```

```csharp
public class BookService
{
    private readonly IMemoryCache _cache;
    private readonly IBookRepository _repository;

    public BookService(IMemoryCache cache, IBookRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _cache.GetOrCreateAsync("books:all", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await _repository.GetAllAsync();
        }) ?? Enumerable.Empty<Book>();
    }

    public void InvalidateCache() => _cache.Remove("books:all");
}
```

⚠️ In-memory caching only works for a **single server instance**. In a load-balanced / multi-instance deployment, each instance has its own separate cache — use distributed caching instead.

## Distributed caching (Redis)

```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "BooksApi:";
});
```

```csharp
public class BookService
{
    private readonly IDistributedCache _cache;

    public async Task<List<Book>?> GetAllAsync()
    {
        var cached = await _cache.GetStringAsync("books:all");
        if (cached is not null)
            return JsonSerializer.Deserialize<List<Book>>(cached);

        var books = await _repository.GetAllAsync();
        await _cache.SetStringAsync("books:all", JsonSerializer.Serialize(books),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

        return books.ToList();
    }
}
```

Redis (or similar) is shared across all instances of your app — the correct choice for anything beyond a single server, and also enables cross-service caching in a microservices setup.

## Response caching (HTTP-level)

```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("BooksPolicy", policy => policy.Expire(TimeSpan.FromMinutes(2)));
});
...
app.UseOutputCache();
```

```csharp
[HttpGet]
[OutputCache(PolicyName = "BooksPolicy")]
public async Task<ActionResult<IEnumerable<Book>>> GetAll() => Ok(await _service.GetAllAsync());
```

This caches the entire HTTP response, which the framework (or a reverse proxy) can serve without even hitting your controller code again.

## Cache invalidation strategy

Cache invalidation is famously one of the two hard problems in computer science. Common approaches:

- **Time-based expiration** — simplest; accept slightly stale data.
- **Explicit invalidation** — remove/update the cache entry whenever the underlying data changes (in `Create`/`Update`/`Delete` handlers).
- **Cache-aside pattern** — application code checks cache first, falls back to the database, then populates the cache (shown above).

## Practice task

Add in-memory caching to `GET /api/books` with a 5-minute sliding expiration, and invalidate the cache entry inside `Create`, `Update`, and `Delete` so clients never see stale data after a write.

**Next → `07-unit-and-integration-testing.md`**
