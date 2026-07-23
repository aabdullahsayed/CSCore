# 7. Async/Await Best Practices in Web APIs

## Why async matters for a Web API

When a request hits an I/O operation (database call, HTTP call to another service, file read), `async`/`await` frees the thread to handle *other incoming requests* instead of blocking. This dramatically improves throughput under load.

```
Synchronous:  Thread waits idle for DB response → wasted capacity
Asynchronous: Thread returns to the pool during DB wait → serves other requests
```

## The golden rule: async all the way

Once you go async at any layer, every caller up the chain should be async too — never mix sync-over-async.

```csharp
// ✅ Good — async end to end
public async Task<ActionResult<Book>> GetById(int id)
{
    var book = await _repo.GetByIdAsync(id);
    return book is null ? NotFound() : Ok(book);
}

// ❌ Bad — blocking on async code causes deadlocks and wastes threads
public ActionResult<Book> GetById(int id)
{
    var book = _repo.GetByIdAsync(id).Result;   // NEVER do this
    return book is null ? NotFound() : Ok(book);
}
```

## Common mistakes

### 1. `async void`
```csharp
// ❌ Never — exceptions can't be caught by the caller, and it's not awaitable
public async void ProcessOrder() { ... }

// ✅ Always return Task (or Task<T>), even if you don't use the result
public async Task ProcessOrder() { ... }
```
`async void` is only acceptable for top-level event handlers, never for API code.

### 2. Not passing CancellationToken
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<Book>>> GetAll(CancellationToken cancellationToken)
{
    return Ok(await _repo.GetAllAsync(cancellationToken));
}

public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken) =>
    await _context.Books.ToListAsync(cancellationToken);
```
ASP.NET Core automatically supplies a `CancellationToken` tied to the request — if the client disconnects, the token is cancelled and EF Core stops the query early, saving database resources.

### 3. Unnecessary `Task.Run` on the server
```csharp
// ❌ Pointless — wraps already-async work in extra overhead, or worse,
// wraps CPU-light work in a thread pool hop for no reason
public async Task<IActionResult> Get()
{
    var result = await Task.Run(() => _repo.GetAllAsync());
    return Ok(result);
}

// ✅ Just await the async method directly
public async Task<IActionResult> Get() => Ok(await _repo.GetAllAsync());
```
`Task.Run` is for offloading genuine CPU-bound work to a background thread — rarely needed in typical CRUD API code, since I/O-bound calls (DB, HTTP) are already async without it.

### 4. Sequential awaits that could run in parallel

```csharp
// ❌ Slower — waits for each one before starting the next
var books = await _repo.GetAllAsync();
var authors = await _authorRepo.GetAllAsync();

// ✅ Faster — both run concurrently
var booksTask = _repo.GetAllAsync();
var authorsTask = _authorRepo.GetAllAsync();
await Task.WhenAll(booksTask, authorsTask);
var books = await booksTask;
var authors = await authorsTask;
```
⚠️ Caveat: don't run multiple queries in parallel against the *same* `DbContext` instance — EF Core's `DbContext` is not thread-safe. Use separate contexts (or scoped `IDbContextFactory`) for true parallel DB access.

## Naming convention

By convention, async methods end with `Async`:
```csharp
Task<Book> GetByIdAsync(int id);
Task SaveChangesAsync();
```

## ConfigureAwait(false) — do you need it?

In ASP.NET Core (unlike classic .NET Framework), there's no `SynchronizationContext` capturing the original thread, so `ConfigureAwait(false)` is **not required** in application code. It's still common in reusable library code, but you can skip it in your controllers/services without issue.

---
**Next:** `08-AutoMapper-And-Clean-Layering.md`
