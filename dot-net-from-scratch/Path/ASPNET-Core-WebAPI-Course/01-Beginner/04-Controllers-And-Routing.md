# 4. Controllers and Routing

## What is a Controller?

A controller is a class that groups related endpoints ("actions") together. It inherits from `ControllerBase` (not `Controller`, which is for MVC views with HTML support you don't need in an API).

```csharp
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    // actions go here
}
```

- `[ApiController]` enables API-specific behaviors: automatic 400 responses on invalid models, binding source inference, etc.
- `[Route("api/[controller]")]` — `[controller]` is a token replaced by the class name minus "Controller", so `BooksController` → `api/books`.

## Attribute routing basics

Each action method is mapped to an HTTP verb + route template using attributes:

```csharp
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    [HttpGet]                         // GET api/books
    public IActionResult GetAll() => Ok(new[] { "Book1", "Book2" });

    [HttpGet("{id}")]                 // GET api/books/5
    public IActionResult GetById(int id) => Ok($"Book #{id}");

    [HttpPost]                        // POST api/books
    public IActionResult Create([FromBody] string title) => Created("", title);

    [HttpPut("{id}")]                 // PUT api/books/5
    public IActionResult Update(int id, [FromBody] string title) => NoContent();

    [HttpDelete("{id}")]              // DELETE api/books/5
    public IActionResult Delete(int id) => NoContent();
}
```

## Route parameters and constraints

```csharp
[HttpGet("{id:int}")]               // only matches numeric id
public IActionResult GetById(int id) => Ok(id);

[HttpGet("{id:int:min(1)}")]        // id must be an integer >= 1
public IActionResult GetPositive(int id) => Ok(id);

[HttpGet("search/{term:alpha}")]    // only alphabetic characters
public IActionResult Search(string term) => Ok(term);
```

Common constraints: `int`, `bool`, `datetime`, `decimal`, `alpha`, `min(x)`, `max(x)`, `length(x)`, `regex(...)`.

## Binding data from the request

| Attribute | Source | Example |
|---|---|---|
| `[FromRoute]` | URL segment | `/api/books/{id}` |
| `[FromQuery]` | Query string | `/api/books?page=2` |
| `[FromBody]` | Request body (JSON) | POST/PUT payloads |
| `[FromHeader]` | HTTP header | `X-Correlation-Id` |
| `[FromForm]` | Form data | File uploads |

```csharp
[HttpGet]
public IActionResult Search(
    [FromQuery] string? title,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    return Ok(new { title, page, pageSize });
}
// GET /api/books?title=dune&page=2
```

> With `[ApiController]`, simple types default to `[FromQuery]`/`[FromRoute]` and complex types default to `[FromBody]` — you often don't need to write the attribute explicitly, but being explicit is more readable, especially for beginners.

## IActionResult vs ActionResult<T> vs concrete types

```csharp
// IActionResult — flexible, any result type
public IActionResult Get() => Ok("data");

// ActionResult<T> — lets you return either T directly or a status result
public ActionResult<Book> Get(int id)
{
    var book = _repo.Find(id);
    if (book is null) return NotFound();
    return book;              // implicit 200 OK with the Book
}

// Common result helpers
Ok(data)                // 200
Created(uri, data)       // 201
NoContent()              // 204
BadRequest(errors)       // 400
NotFound()               // 404
Unauthorized()           // 401
Forbid()                 // 403
Conflict()               // 409
StatusCode(500, error)   // custom status
```

Prefer `ActionResult<T>` for typed endpoints — it gives you both Swagger documentation of the return type AND the flexibility to return error status codes.

## Route grouping / prefixes

```csharp
[Route("api/v1/[controller]")]
public class BooksController : ControllerBase { ... }
```

You can also nest resources:

```csharp
[HttpGet("{bookId}/reviews")]        // GET api/books/5/reviews
public IActionResult GetReviews(int bookId) => Ok();
```

---
**Next:** `05-HTTP-Methods-CRUD.md` — building a full CRUD controller.
