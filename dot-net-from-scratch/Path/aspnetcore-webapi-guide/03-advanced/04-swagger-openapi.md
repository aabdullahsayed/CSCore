# 4. Swagger / OpenAPI

## What it gives you

Swagger (via **Swashbuckle**) generates an interactive, browsable API documentation UI directly from your code — including request/response schemas, and a "Try it out" button to call endpoints live.

## Setup (already included in the default template)

```bash
dotnet add package Swashbuckle.AspNetCore
```

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Books API",
        Version = "v1",
        Description = "A sample API for managing books",
        Contact = new OpenApiContact { Name = "API Support", Email = "support@example.com" }
    });

    // Include XML comments for richer descriptions
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Books API v1"));
}
```

Enable XML doc generation in the `.csproj`:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

## Documenting endpoints

```csharp
/// <summary>Gets a book by its unique id.</summary>
/// <param name="id">The book's id.</param>
/// <response code="200">Returns the requested book.</response>
/// <response code="404">If the book doesn't exist.</response>
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<BookResponseDto>> GetById(int id) { ... }
```

## Adding JWT support to Swagger UI

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
        Description = "Enter your JWT token below."
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

This adds an "Authorize" button in Swagger UI where you can paste a token and have it automatically attached to every request you try.

## Versioned Swagger docs

When combined with API versioning (previous file), register one Swagger doc per version:

```csharp
options.SwaggerDoc("v1", new OpenApiInfo { Title = "Books API", Version = "v1" });
options.SwaggerDoc("v2", new OpenApiInfo { Title = "Books API", Version = "v2" });
```

```csharp
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Books API v1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Books API v2");
});
```

## Alternative: the built-in .NET 9+ OpenAPI document generator

Newer .NET versions include `Microsoft.AspNetCore.OpenApi` for generating an OpenAPI document without Swashbuckle, often paired with a separate UI like **Scalar**. Swashbuckle remains the most widely used choice today.

## Practice task

Add XML doc comments and `[ProducesResponseType]` attributes to every action in your Books API, then confirm the descriptions and response codes render correctly in Swagger UI.

**Next → `05-global-exception-handling.md`**
