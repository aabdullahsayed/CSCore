# 3. Your First Web API Project

## 1. Scaffold the project

```bash
dotnet new webapi -n BookStore.Api --use-controllers
cd BookStore.Api
```

> `--use-controllers` ensures you get the classic controller-based template (rather than the newer "minimal API" template). We teach controllers first because they scale better for larger APIs; minimal APIs are covered as a comparison at the end of this file.

## 2. Project structure

```
BookStore.Api/
├── Controllers/
│   └── WeatherForecastController.cs   ← sample controller (delete later)
├── Properties/
│   └── launchSettings.json            ← local run/debug config
├── appsettings.json                   ← configuration (connection strings, etc.)
├── appsettings.Development.json       ← dev-only overrides
├── Program.cs                         ← app entry point & configuration
├── WeatherForecast.cs                 ← sample model
└── BookStore.Api.csproj               ← project file (dependencies, target framework)
```

## 3. Run it

```bash
dotnet run
```

You'll see output like:

```
Now listening on: https://localhost:7031
Now listening on: http://localhost:5215
```

Open `https://localhost:7031/swagger` in your browser — you'll see **Swagger UI**, an interactive page listing your API's endpoints, generated automatically from your code.

## 4. Understanding `Program.cs`

This is the minimal hosting model — everything starts here:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container (dependency injection).
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline (middleware).
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

Two phases:
1. **`builder.Services.Add...()`** — register services into the DI container (before `Build()`).
2. **`app.Use...()` / `app.Map...()`** — configure the middleware pipeline (after `Build()`).

## 5. Your first custom endpoint

Delete `WeatherForecastController.cs` and `WeatherForecast.cs` (or keep them for reference), then create `Controllers/HelloController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]   // becomes: api/hello
public class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Hello, ASP.NET Core!" });
    }
}
```

Run again (`dotnet run` or better, `dotnet watch run` for auto-reload), then visit:

```
GET https://localhost:7031/api/hello
```

Response:
```json
{ "message": "Hello, ASP.NET Core!" }
```

## Minimal APIs — a quick comparison

.NET also supports defining endpoints without controllers, directly in `Program.cs`:

```csharp
app.MapGet("/api/hello", () => Results.Ok(new { message = "Hello, Minimal API!" }));
```

Minimal APIs are great for tiny services or microservices with few endpoints. **This course uses controllers** because they organize better as an API grows (attribute routing, filters, model binding conventions, etc.), but everything you learn transfers to minimal APIs too.

---
**Next:** `04-Controllers-And-Routing.md`
