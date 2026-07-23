# 4. Docker and Deployment

## Why containerize?

Docker packages your app with everything it needs to run, guaranteeing "it works the same on my machine, in CI, and in production." It's the standard deployment unit for cloud platforms and Kubernetes.

## A production-ready Dockerfile (multi-stage build)

```dockerfile
# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MyApi/MyApi.csproj", "MyApi/"]
RUN dotnet restore "MyApi/MyApi.csproj"

COPY . .
WORKDIR /src/MyApi
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

# Run as non-root for security
USER $APP_UID

ENTRYPOINT ["dotnet", "MyApi.dll"]
```

**Why multi-stage?** The SDK image (needed to compile) is large; the final image only needs the smaller ASP.NET runtime image plus your compiled output — keeping the deployed image lean and reducing attack surface.

## Build and run locally

```bash
docker build -t myapi:latest .
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production myapi:latest
```

## docker-compose for local multi-service development

```yaml
version: "3.8"
services:
  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__Default=Server=db;Database=BooksDb;User=sa;Password=Your_password123!
    depends_on:
      - db
      - redis

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_password123!
    ports:
      - "1433:1433"

  redis:
    image: redis:alpine
    ports:
      - "6379:6379"
```

```bash
docker compose up --build
```

## Health checks

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

app.MapHealthChecks("/health");
```

Deployment platforms (Kubernetes, Azure App Service, ECS) poll `/health` to know when to route traffic to a new instance and when to restart an unhealthy one.

## CI/CD with GitHub Actions

```yaml
name: build-and-deploy
on:
  push:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "8.0.x"
      - run: dotnet restore
      - run: dotnet build --no-restore -c Release
      - run: dotnet test --no-build -c Release
      - name: Build and push Docker image
        run: |
          docker build -t myregistry.azurecr.io/myapi:${{ github.sha }} .
          docker push myregistry.azurecr.io/myapi:${{ github.sha }}
```

A typical pipeline: **restore → build → test → build image → push to registry → deploy**. Gate deployment on tests passing.

## Deployment targets

| Platform | Notes |
|---|---|
| **Azure App Service** | Simplest managed hosting; built-in deployment slots for blue-green deploys |
| **Azure Container Apps / AWS ECS Fargate** | Serverless containers, good middle ground |
| **Kubernetes (AKS/EKS/GKE)** | Full control, best for complex microservice fleets |
| **Render / Railway / Fly.io** | Fast, simple for smaller projects and side projects |

## Configuration in containers

Never bake secrets into the image. Inject via environment variables (as in the compose file above) or a secrets manager (Azure Key Vault, AWS Secrets Manager) — ASP.NET Core's configuration system picks up environment variables automatically, using `__` (double underscore) instead of `:` for nested keys, e.g. `ConnectionStrings__Default`.

## Practice task

Write a multi-stage Dockerfile for your Books API, add a `/health` endpoint checking the database connection, and create a `docker-compose.yml` running the API alongside SQL Server and Redis.

**Next → `05-performance-optimization.md`**
