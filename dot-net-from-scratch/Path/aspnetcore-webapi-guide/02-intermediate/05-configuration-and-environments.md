# 5. Configuration and Environments

## Configuration sources (in priority order, later overrides earlier)

1. `appsettings.json`
2. `appsettings.{Environment}.json` (e.g., `appsettings.Development.json`)
3. Environment variables
4. Command-line arguments
5. User Secrets (Development only)

## appsettings.json example

```json
{
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\mssqllocaldb;Database=BooksDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "Issuer": "MyApi",
    "Audience": "MyApiUsers",
    "ExpiryMinutes": 60
  },
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  }
}
```

## Reading configuration directly

```csharp
var connectionString = builder.Configuration.GetConnectionString("Default");
var issuer = builder.Configuration["Jwt:Issuer"];
```

## Strongly-typed configuration with `IOptions<T>`

```csharp
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
}
```

```csharp
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
```

```csharp
public class TokenService
{
    private readonly JwtSettings _settings;
    public TokenService(IOptions<JwtSettings> options) => _settings = options.Value;
}
```

- `IOptions<T>` — resolved once, singleton-friendly.
- `IOptionsSnapshot<T>` — re-read per scope, useful in Scoped services if config might change.
- `IOptionsMonitor<T>` — supports live reload notifications, ideal in Singletons.

## Environments

ASP.NET Core reads the `ASPNETCORE_ENVIRONMENT` variable (`Development`, `Staging`, `Production`).

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
```

Set it locally via `launchSettings.json` or:

```bash
export ASPNETCORE_ENVIRONMENT=Development   # Linux/macOS
$env:ASPNETCORE_ENVIRONMENT="Development"   # PowerShell
```

## User Secrets (never commit real secrets to source control)

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "super-secret-signing-key"
```

These are stored outside the project folder and automatically merged into configuration during Development.

## Practice task

Move the SQL connection string and JWT settings into `appsettings.json`, create a strongly-typed `JwtSettings` class bound via `IOptions<T>`, and store a fake secret key using `dotnet user-secrets`.

**Next → `06-logging.md`**
