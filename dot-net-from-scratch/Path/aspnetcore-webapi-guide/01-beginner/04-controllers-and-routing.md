# 4. Controllers and Routing

## Attribute routing basics

```csharp
[ApiController]
[Route("api/[controller]")]   // -> api/products
public class ProductsController : ControllerBase
{
    [HttpGet]                       // GET api/products
    public IActionResult GetAll() => Ok();

    [HttpGet("{id:int}")]           // GET api/products/5
    public IActionResult GetById(int id) => Ok();

    [HttpPost]                      // POST api/products
    public IActionResult Create([FromBody] object dto) => Ok();

    [HttpPut("{id:int}")]           // PUT api/products/5
    public IActionResult Update(int id, [FromBody] object dto) => Ok();

    [HttpDelete("{id:int}")]        // DELETE api/products/5
    public IActionResult Delete(int id) => Ok();
}
```

- `[controller]` is a token replaced with the class name minus "Controller".
- Route constraints like `{id:int}` restrict the parameter type, so `/products/abc` won't match.

## Common route constraints

| Constraint | Example | Matches |
|---|---|---|
| `int` | `{id:int}` | `5`, `-3` |
| `bool` | `{active:bool}` | `true`, `false` |
| `guid` | `{id:guid}` | a GUID string |
| `alpha` | `{name:alpha}` | letters only |
| `min(x)`/`max(x)` | `{id:min(1)}` | numeric range |
| `regex(...)` | `{code:regex(^[A-Z]{{3}}$)}` | custom pattern |

## Binding sources — where do parameters come from?

```csharp
[HttpGet]
public IActionResult Search(
    [FromQuery] string term,        // ?term=abc
    [FromRoute] int id,             // /products/{id}
    [FromBody] ProductDto dto,      // JSON request body
    [FromHeader(Name = "X-Client")] string client)
{
    ...
}
```

With `[ApiController]`, ASP.NET Core infers most of these automatically: complex types default to `[FromBody]`, simple types default to `[FromRoute]`/`[FromQuery]`. Explicit attributes are still good practice for clarity.

## Returning results

`ControllerBase` provides helper methods that map to HTTP status codes:

```csharp
return Ok(product);              // 200
return Ok();                     // 200, no body
return CreatedAtAction(nameof(GetById), new { id = product.Id }, product); // 201
return NoContent();               // 204
return BadRequest("Invalid id");  // 400
return NotFound();                // 404
return Conflict();                // 409
return StatusCode(500, "Oops");   // custom code
```

Prefer returning `ActionResult<T>` so both the object and status codes are strongly typed:

```csharp
[HttpGet("{id:int}")]
public ActionResult<Product> GetById(int id)
{
    var product = _repo.Find(id);
    if (product is null) return NotFound();
    return Ok(product);
}
```

## Route groups (multiple routes per action)

```csharp
[HttpGet]
[HttpGet("all")]   // both api/products and api/products/all work
public IActionResult GetAll() => Ok();
```

## Minimal APIs — the alternative style

Since .NET 6+, you can skip controllers entirely and define endpoints directly in `Program.cs`:

```csharp
app.MapGet("/api/products", () => products);
app.MapGet("/api/products/{id:int}", (int id) =>
    products.FirstOrDefault(p => p.Id == id) is { } p ? Results.Ok(p) : Results.NotFound());
app.MapPost("/api/products", (Product p) => { products.Add(p); return Results.Created($"/api/products/{p.Id}", p); });
```

Minimal APIs are great for small services and microservices; controllers scale better for larger, more structured APIs. This guide primarily uses controllers, but everything you learn (DI, middleware, EF Core, auth) applies to both styles.

## Practice task

Build a `BooksController` with in-memory `List<Book>` and full CRUD (`GET all`, `GET by id`, `POST`, `PUT`, `DELETE`), returning correct status codes for each case (including 404 when not found).

**Next → `05-http-methods-and-crud.md`**
