# 8. Docker and Deployment

## Why containerize your API?

Docker packages your app plus its exact runtime environment into a portable image, guaranteeing "works on my machine" also means "works in production" — identical behavior across dev, staging, and prod.

## A production-ready Dockerfile (multi-stage build)

```dockerfile
# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BookStore.Api.csproj", "./"]
RUN dotnet restore "BookStore.Api.csproj"

COPY . .
RUN dotnet publish "BookStore.Api.csproj" -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Run as non-root user for security
RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookStore.Api.dll"]
```

**Why multi-stage?** The SDK image (~800MB, includes compilers/build tools) is only needed to *build* the app. The final image uses the much smaller ASP.NET runtime image (~200MB) and contains only the compiled output — smaller, faster to pull, smaller attack surface.

## .dockerignore

```
bin/
obj/
*.user
.vs/
.git/
**/Migrations/*.Designer.cs.bak
```

## Building and running

```bash
docker build -t bookstore-api:latest .
docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="..." bookstore-api:latest
```

Note: environment variable names use `__` (double underscore) to represent the `:` nesting in `appsettings.json` (`ConnectionStrings:DefaultConnection` → `ConnectionStrings__DefaultConnection`).

## docker-compose for local development (API + database)

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=bookstore;Username=postgres;Password=postgres
      - ASPNETCORE_ENVIRONMENT=Development
    depends_on:
      - db

  db:
    image: postgres:16
    environment:
      - POSTGRES_DB=bookstore
      - POSTGRES_PASSWORD=postgres
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:
```

```bash
docker compose up --build
```

## Applying EF Core migrations in production

Two common patterns:

**1. Run migrations on startup** (simple, fine for small apps):
```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}
```

**2. Run migrations as a separate CI/CD step** (recommended for production — avoids race conditions when scaling to multiple instances):
```bash
dotnet ef database update --connection "<production-connection-string>"
```

## Configuration per environment

```
appsettings.json                  → base config, safe defaults
appsettings.Development.json      → local dev overrides
appsettings.Production.json       → production overrides (often just feature flags; secrets come from env vars/vault)
```

Set the active environment via `ASPNETCORE_ENVIRONMENT` (`Development`, `Staging`, `Production`).

## Deployment targets — a quick map

| Target | Good for |
|---|---|
| **Azure App Service** | Easiest managed deployment for .NET, built-in CI/CD from GitHub |
| **AWS Elastic Beanstalk / ECS** | AWS-native managed deployment |
| **Kubernetes (AKS/EKS/GKE)** | Large-scale, multi-service, need fine-grained orchestration |
| **Railway / Render / Fly.io** | Simple, low-friction container hosting for smaller projects |
| **Bare VM + reverse proxy (nginx)** | Full control, more manual ops work |

## Health checks (needed by most orchestrators)

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

app.MapHealthChecks("/health");
```

Load balancers and Kubernetes probes hit `/health` to know whether to route traffic to this instance.

## CI/CD — minimal GitHub Actions example

```yaml
name: CI
on: [push]
jobs:
  build-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build
```

---
**Next:** `09-Performance-Best-Practices.md`
