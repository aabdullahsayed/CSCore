# 5. API Versioning

## Why version your API?

Once clients depend on your API, you can't freely make breaking changes (renaming fields, changing response shapes, removing endpoints) without versioning — it would break every consumer. Versioning lets old and new clients coexist while you evolve the API.

## Install the package

```bash
dotnet add package Asp.Versioning.Mvc
dotnet add package Asp.Versioning.Mvc.ApiExplorer
```

## Configure versioning

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;               // adds api-supported-versions response header
    options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
        new Asp.Versioning.UrlSegmentApiVersionReader(),
        new Asp.Versioning.HeaderApiVersionReader("X-Api-Version")
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
```

## Versioning strategies compared

| Strategy | Example | Pros | Cons |
|---|---|---|---|
| URL segment | `/api/v1/books` | Explicit, cacheable, easy to test in a browser | "Pollutes" the URL |
| Query string | `/api/books?api-version=1.0` | Doesn't change the path | Easy to forget/omit |
| Header | `X-Api-Version: 1.0` | Clean URLs | Less discoverable, harder to test manually |
| Media type | `Accept: application/json;v=1.0` | Very RESTful | Rarely used in practice, more complex |

**URL segment versioning is the most common and easiest to teach/debug** — this course uses it.

## Applying versions to controllers

```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(new[] { "Book1", "Book2" });
}
```

## Running two versions side by side

When v2 introduces a breaking change (e.g., renamed field), create a separate controller:

```csharp
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class BooksV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(new[] {
        new { title = "Book1", publishedYear = 2020 }   // new field added in v2
    });
}
```

Or version individual actions within the same controller when only some endpoints change:

```csharp
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BooksController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public IActionResult GetAllV1() => Ok(/* old shape */);

    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetAllV2() => Ok(/* new shape */);
}
```

## Deprecating a version

```csharp
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
```

Deprecated versions still work but signal to clients (via the `api-supported-versions` / `api-deprecated-versions` response headers) that they should migrate.

## Practical advice

- Start with `v1` even for your very first release — retrofitting versioning later is painful.
- Only introduce a new version for genuinely **breaking** changes. Adding a new optional field to a response is *not* breaking — no new version needed.
- Document a deprecation policy (e.g., "old versions supported for 6 months after a new version ships") and communicate it clearly.

---
**Next:** `06-Swagger-OpenAPI.md`
