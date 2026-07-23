# Cheatsheet & Further Reading

## HTTP status codes quick reference

| Code | Name | Use |
|---|---|---|
| 200 | OK | Successful GET/PUT |
| 201 | Created | Successful POST |
| 204 | No Content | Successful action with no body to return |
| 400 | Bad Request | Invalid input / validation failure |
| 401 | Unauthorized | Missing/invalid authentication |
| 403 | Forbidden | Authenticated but not permitted |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | State conflict (e.g., duplicate resource) |
| 422 | Unprocessable Entity | Semantically invalid data (alternative to 400) |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Unhandled server-side failure |
| 503 | Service Unavailable | Server temporarily overloaded/down |

## dotnet CLI cheat sheet

```bash
dotnet new webapi -n MyApi --use-controllers   # scaffold project
dotnet run                                     # run
dotnet watch run                               # run with hot reload
dotnet build                                   # compile
dotnet test                                    # run tests
dotnet add package <Name>                      # install NuGet package
dotnet ef migrations add <Name>                # create migration
dotnet ef database update                      # apply migrations
dotnet publish -c Release -o ./publish         # production build
```

## Project layout reference (a mature project)

```
BookStore.Api/
├── Controllers/
├── Dtos/
│   ├── Requests/
│   └── Responses/
├── Models/                 (Entities)
├── Data/
│   └── AppDbContext.cs
├── Repositories/
├── Services/
├── Middleware/
├── Filters/
├── Validators/
├── Mappings/
├── Migrations/
├── Program.cs
└── appsettings.json

BookStore.Api.Tests/
├── UnitTests/
└── IntegrationTests/
```

## Checklist before shipping to production

- [ ] Secrets are in environment variables / a secret manager, never in source control
- [ ] `[ApiController]` validation + global exception handling in place
- [ ] Authentication + authorization on every non-public endpoint
- [ ] HTTPS enforced (`UseHttpsRedirection`)
- [ ] CORS restricted to known origins (not `AllowAnyOrigin` in production)
- [ ] Structured logging configured (Serilog or similar)
- [ ] Health check endpoint exposed for your orchestrator/load balancer
- [ ] Rate limiting on public-facing endpoints
- [ ] Database migrations tested against a staging environment first
- [ ] Pagination on any endpoint returning a potentially large collection
- [ ] API versioned from day one
- [ ] Swagger/OpenAPI docs accurate and up to date
- [ ] Unit + integration tests passing in CI before merge
- [ ] Response compression enabled
- [ ] Sensitive fields (passwords, tokens) never appear in logs or responses

## Official documentation (bookmark these)

- ASP.NET Core docs: https://learn.microsoft.com/aspnet/core
- EF Core docs: https://learn.microsoft.com/ef/core
- C# language reference: https://learn.microsoft.com/dotnet/csharp
- ASP.NET Core GitHub (source + issues): https://github.com/dotnet/aspnetcore

## Suggested next topics once you finish this course

- GraphQL with HotChocolate (alternative to REST)
- gRPC for high-performance internal service communication
- Background jobs with Hosted Services / Quartz.NET / Hangfire
- SignalR for real-time features (websockets)
- Domain-Driven Design (DDD) and CQRS patterns for complex business domains
- Event sourcing

## A note on staying current

.NET ships a major version every year (LTS every other year). APIs and best practices in this course reflect .NET 8 conventions as of writing — always cross-check against the official docs above for anything version-specific before shipping to production.
