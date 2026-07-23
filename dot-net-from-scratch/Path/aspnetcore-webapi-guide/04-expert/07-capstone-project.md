# 7. Capstone Project

You've covered everything from your first controller to microservices and security hardening. Time to build something end-to-end that ties it all together.

## Project: "Library Management API"

Build a production-style API for a library system, applying everything from this guide.

### Core domain

- **Books** — title, author, ISBN, price, available copies
- **Members** — registered library users
- **Loans** — a member borrowing a book, with a due date and return status
- **Reviews** — members can rate/review books they've borrowed

### Functional requirements

1. **Auth** — Register/login with JWT; roles: `Member`, `Librarian`, `Admin`.
2. **Books CRUD** — Librarians/Admins can create/update/delete; anyone can browse/search (paginated, filterable by author/title).
3. **Loans** — Members can borrow an available book (decrement available copies); returning a book increments it back. Prevent borrowing when no copies are available (`409 Conflict`).
4. **Reviews** — Members can review a book only if they have a completed loan for it (resource-based authorization).
5. **Admin dashboard endpoint** — aggregate stats: total books, active loans, overdue loans.

### Technical requirements checklist

- [ ] **Clean Architecture** — `Domain`, `Application`, `Infrastructure`, `Api` projects
- [ ] **CQRS + MediatR** — separate Commands and Queries for every use case
- [ ] **EF Core + migrations** — real database (SQL Server or PostgreSQL), proper relationships and indexes
- [ ] **DTOs + FluentValidation** — no entities exposed directly, all input validated
- [ ] **JWT auth + role/policy-based authorization** — including resource-based checks for reviews
- [ ] **Global exception handling** — consistent `ProblemDetails` responses
- [ ] **Structured logging** — Serilog, with correlation IDs
- [ ] **Caching** — cache the book catalog listing; invalidate on writes
- [ ] **API versioning** — `v1` in place, structured to support a future `v2`
- [ ] **Swagger** — fully documented, with JWT support enabled in the UI
- [ ] **Rate limiting** — protect the login endpoint
- [ ] **Unit tests** — for domain logic and command/query handlers
- [ ] **Integration tests** — for at least the auth flow and the loan-borrowing flow (including the "no copies available" edge case)
- [ ] **Dockerized** — multi-stage Dockerfile + docker-compose with the database and Redis
- [ ] **CI pipeline** — GitHub Actions running build, test, and Docker build on every push
- [ ] **Health check endpoint** — `/health` checking DB and cache connectivity
- [ ] **Security pass** — DTOs everywhere, security headers, dependency vulnerability scan

### Suggested build order

1. Domain entities and business rules (no framework dependencies)
2. Infrastructure: DbContext, migrations, repositories
3. Application layer: Commands/Queries + handlers + validators
4. API layer: controllers wired to MediatR, Swagger, versioning
5. Auth: registration, login, JWT issuance, role/policy authorization
6. Cross-cutting: logging, exception handling, caching, rate limiting
7. Tests: unit tests for handlers, integration tests for critical flows
8. Containerize and set up CI
9. Security review pass using the OWASP checklist from the previous file

### Stretch goals (once the core is solid)

- Add a **Notifications** microservice that consumes a `LoanOverdueEvent` and (mock) sends an email
- Add **output caching** on the book catalog endpoint alongside the in-memory cache
- Add **OpenTelemetry** tracing and export to a local Jaeger/Zipkin instance to visualize request flows
- Deploy to a free-tier cloud platform (Render, Fly.io, Azure free tier) with environment-based configuration

## You're done — what now?

At this point you have working knowledge of the full ASP.NET Core Web API stack, from a single controller to a hardened, tested, containerized, versioned, and observable service. From here, the best way to keep growing is to build and ship real projects, read the official Microsoft Learn docs for anything that gets deprecated or changed, and study open-source ASP.NET Core codebases to see these patterns applied at scale.

Good luck — go build something.
