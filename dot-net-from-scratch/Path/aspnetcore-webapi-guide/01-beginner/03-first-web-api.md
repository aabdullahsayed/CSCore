# 3. Your First Web API

## Create the project

```bash
dotnet new webapi -n MyFirstApi --use-controllers
cd MyFirstApi
dotnet run
```

The console will show something like:

```
Now listening on: https://localhost:7123
Now listening on: http://localhost:5123
```

Open `https://localhost:7123/swagger` in a browser to see the auto-generated Swagger UI, or hit `https://localhost:7123/weatherforecast` directly.

## Anatomy of the generated project

```
MyFirstApi/
├── Controllers/
│   └── WeatherForecastController.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── MyFirstApi.csproj
```

### `Program.cs` — the entry point

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

This is the **minimal hosting model**: everything — service registration and the middleware pipeline — lives in one file. Two phases:

1. **Before `builder.Build()`** — register services into the DI container.
2. **After `builder.Build()`** — configure the middleware pipeline (order matters here).

### A sample controller

```csharp
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = "Mild"
        });
    }
}
```

- `[ApiController]` enables automatic model validation, binding source inference, and problem-details error responses.
- `[Route("[controller]")]` maps this controller to `/weatherforecast` (the word "Controller" is stripped).
- `ControllerBase` (not `Controller`) is the correct base class for APIs — `Controller` adds view-rendering features you don't need.

## Run and hot-reload

```bash
dotnet watch run
```

Now edit the controller and save — the app rebuilds and the browser (if open) refreshes automatically.

## Practice task

1. Add a new `HelloController` with a `GET /hello` endpoint that returns `"Hello, ASP.NET Core!"`.
2. Confirm it shows up in Swagger UI.

**Next → `04-controllers-and-routing.md`**
