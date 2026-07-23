# 3. API Versioning

## Why version your API?

Once clients depend on your API, breaking changes (removing a field, changing a response shape) will break them. Versioning lets you evolve the API while keeping old clients working.

## Install the package

```bash
dotnet add package Asp.Versioning.Mvc
dotnet add package Asp.Versioning.Mvc.ApiExplorer
```

## Configure versioning

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"),
        new QueryStringApiVersionReader("api-version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
```

## Versioning strategies

| Strategy | Example | Pros / Cons |
|---|---|---|
| **URL segment** | `/api/v1/books` | Most explicit, easiest to discover; changes the URL |
| **Query string** | `/api/books?api-version=1.0` | Doesn't touch the path; easy to miss |
| **Header** | `X-Api-Version: 1.0` | Clean URLs; less discoverable |
| **Media type** | `Accept: application/json;v=1.0` | RESTfully "correct"; more complex to implement |

URL segment versioning is the most common and beginner-friendly choice.

## Versioned controllers

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BooksController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(/* v1 shape */);
}

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BooksV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(/* v2 shape, e.g. added pagination */);
}
```

## Deprecating a version gracefully

```csharp
[ApiVersion("1.0", Deprecated = true)]
```

Deprecated versions still work but are flagged in Swagger and the `api-supported-versions` response header, signaling clients to migrate.

## Practice task

Add `v2` of the Books API where `GET /api/v2/books` returns a paginated response (`{ items, totalCount, page, pageSize }`) instead of a raw array, while `v1` keeps returning the raw array unchanged.

**Next → `04-swagger-openapi.md`**
