# 7. Async/Await in Web APIs

## Why async matters for APIs

ASP.NET Core uses a limited thread pool to handle requests. If a controller action blocks a thread while waiting on I/O (database call, HTTP call, file read), that thread can't serve other requests — under load, this exhausts the thread pool and tanks throughput.

`async`/`await` frees the thread during I/O waits, letting it serve other requests in the meantime.

## Basic pattern

```csharp
[HttpGet("{id:int}")]
public async Task<ActionResult<Book>> GetById(int id)
{
    var book = await _context.Books.FindAsync(id);   // thread released during DB call
    return book is null ? NotFound() : Ok(book);
}
```

## Rules of thumb

- If a method calls an `async` method, make it `async` too and `await` the call — "async all the way."
- Never use `.Result` or `.Wait()` on a `Task` in a web app — this can cause **deadlocks**.
- Use the `Async` suffix by convention: `GetByIdAsync`, `SaveChangesAsync`.
- EF Core, `HttpClient`, and most I/O APIs already expose async versions — use them.

## Bad vs good

```csharp
// ❌ Blocks a thread pool thread, risks deadlock
public Book GetById(int id) => _context.Books.Find(id);

// ✅ Frees the thread during the wait
public async Task<Book?> GetByIdAsync(int id) => await _context.Books.FindAsync(id);
```

## Running independent tasks concurrently

```csharp
[HttpGet("dashboard")]
public async Task<IActionResult> GetDashboard()
{
    var booksTask = _bookService.GetAllAsync();
    var authorsTask = _authorService.GetAllAsync();

    await Task.WhenAll(booksTask, authorsTask);

    return Ok(new { Books = booksTask.Result, Authors = authorsTask.Result });
}
```

This runs both calls concurrently instead of sequentially — useful when calls are independent (e.g., two different services or external APIs).

## Cancellation tokens

Long-running operations should respect cancellation, especially if the client disconnects:

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<Book>>> GetAll(CancellationToken cancellationToken)
{
    var books = await _context.Books.ToListAsync(cancellationToken);
    return Ok(books);
}
```

ASP.NET Core automatically supplies a `CancellationToken` tied to the request; you don't need to create one manually — just accept it as a parameter and pass it through.

## Common pitfalls

| Pitfall | Fix |
|---|---|
| `async void` methods (except event handlers) | Use `async Task` instead — exceptions in `async void` can crash the process |
| Mixing blocking and async code (`.Result`, `.Wait()`) | Await everything |
| Wrapping already-async code in `Task.Run()` unnecessarily | Only use `Task.Run` for CPU-bound work, not I/O |
| Forgetting `ConfigureAwait(false)` in libraries | Not needed in ASP.NET Core apps (no `SynchronizationContext`), but still relevant in shared libraries |

## Practice task

Convert every repository and controller method in your Books API to be fully async, and add a `CancellationToken` parameter to the `GetAll` endpoint.

## Intermediate section checklist

- [ ] I understand service lifetimes (Transient/Scoped/Singleton) and can spot captive dependency bugs
- [ ] I can connect to a real database with EF Core and run migrations
- [ ] I use DTOs with validation attributes instead of exposing entities directly
- [ ] I understand middleware ordering and can write custom middleware
- [ ] I use strongly-typed configuration via `IOptions<T>`
- [ ] I use structured logging with `ILogger<T>`
- [ ] I write fully async controllers and repositories

**Next → `../03-advanced/01-authentication-jwt.md`**
