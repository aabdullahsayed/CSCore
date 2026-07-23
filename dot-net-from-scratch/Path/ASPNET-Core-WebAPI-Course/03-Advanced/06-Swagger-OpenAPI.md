# 6. Swagger / OpenAPI Documentation

## What is OpenAPI vs Swagger?

- **OpenAPI** is the specification (a JSON/YAML document describing your API's endpoints, parameters, and schemas).
- **Swagger** is the tooling ecosystem (Swashbuckle for .NET generates the spec; Swagger UI renders it as an interactive web page).

The default Web API template already includes `Swashbuckle.AspNetCore` — you saw this in `01-Beginner/03-First-WebAPI-Project.md`.

## Enhancing the basic setup

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookStore API",
        Version = "v1",
        Description = "An API for managing a bookstore's catalog.",
        Contact = new OpenApiContact { Name = "API Support", Email = "support@example.com" }
    });

    // Include XML comments (see below) for richer descriptions
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
```

Enable XML doc generation in the `.csproj`:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

## Documenting endpoints with XML comments

```csharp
/// <summary>
/// Retrieves a single book by its unique identifier.
/// </summary>
/// <param name="id">The book's unique ID.</param>
/// <response code="200">The book was found.</response>
/// <response code="404">No book exists with the given ID.</response>
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<BookResponseDto>> GetById(int id) { ... }
```

`[ProducesResponseType]` tells Swagger UI exactly which status codes and shapes an endpoint can return — this makes the generated docs (and any client SDK generated from them) far more accurate.

## Documenting JWT authentication in Swagger

So you can click "Authorize" in Swagger UI and test protected endpoints directly:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token like: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
```

## Grouping endpoints by tag

```csharp
[ApiExplorerSettings(GroupName = "Catalog")]
[Route("api/[controller]")]
public class BooksController : ControllerBase { ... }
```

Or simply rely on the default grouping by controller name — Swagger UI automatically groups actions under their controller.

## Versioned Swagger docs (pairs with the previous file)

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BookStore API", Version = "v1" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "BookStore API", Version = "v2" });
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
});
```

## Alternative: Scalar / built-in .NET 9 OpenAPI

Newer .NET versions ship `Microsoft.AspNetCore.OpenApi` as a lighter-weight, built-in alternative to Swashbuckle, often paired with the **Scalar** UI instead of classic Swagger UI. Both approaches (Swashbuckle+Swagger UI, or `Microsoft.AspNetCore.OpenApi`+Scalar) are valid — check current .NET docs for your specific SDK version, since this area evolves quickly.

## Why bother with thorough docs?

Good Swagger docs double as:
- A live, always-up-to-date testing playground for developers.
- A contract front-end teams can build against before your implementation is even finished.
- The source for auto-generated client SDKs (e.g., via `NSwag` or `openapi-generator`).

---
**Next:** `07-Unit-And-Integration-Testing.md`
