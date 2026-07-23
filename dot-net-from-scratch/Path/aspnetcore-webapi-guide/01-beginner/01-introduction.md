# 1. Introduction to ASP.NET Core Web API

## What is a Web API?

A **Web API** is a server application that exposes data and functionality over HTTP, typically consumed by web front-ends, mobile apps, or other services. Instead of returning HTML pages, it returns data — usually as **JSON**.

```
Client (React / Angular / Mobile app / Postman)
        │  HTTP request (GET, POST, PUT, DELETE...)
        ▼
   ASP.NET Core Web API
        │  reads/writes
        ▼
     Database
```

## What is ASP.NET Core?

ASP.NET Core is a free, open-source, cross-platform framework from Microsoft for building web apps and APIs. Key traits:

- **Cross-platform** — runs on Windows, Linux, macOS.
- **High performance** — one of the fastest mainstream web frameworks (see TechEmpower benchmarks).
- **Modular** — you only include the middleware/packages you need.
- **Built-in dependency injection**, logging, configuration.
- **Unified** — MVC, Web API, Razor Pages, Blazor, and gRPC all share the same underlying pipeline.

## Why build APIs with it?

- Strong typing with C#
- Excellent tooling (Visual Studio, Rider, VS Code)
- First-class support for JSON, OpenAPI/Swagger, JWT auth, EF Core
- Huge ecosystem via NuGet
- Great for microservices due to small footprint and fast startup

## Core building blocks you'll learn

| Concept | Purpose |
|---|---|
| **Controller** | Handles incoming HTTP requests |
| **Route** | Maps a URL pattern to a controller action |
| **Model** | Represents the shape of your data |
| **Middleware** | Code that runs on every request/response (logging, auth, error handling) |
| **Dependency Injection** | Supplies services (like a database context) to your classes |
| **DbContext (EF Core)** | Talks to the database |

## REST — the design style behind most Web APIs

REST (Representational State Transfer) is a set of conventions:

- Resources are nouns: `/api/products`, not `/api/getProducts`
- HTTP verbs describe the action: `GET`, `POST`, `PUT`, `PATCH`, `DELETE`
- Responses use standard HTTP status codes: `200 OK`, `201 Created`, `404 Not Found`, `400 Bad Request`, etc.
- Stateless: each request contains everything needed to process it (no server-side session state)

Example REST mapping for a `Product` resource:

| Verb | URL | Action |
|---|---|---|
| GET | `/api/products` | Get all products |
| GET | `/api/products/5` | Get product with id 5 |
| POST | `/api/products` | Create a new product |
| PUT | `/api/products/5` | Replace product 5 entirely |
| PATCH | `/api/products/5` | Partially update product 5 |
| DELETE | `/api/products/5` | Delete product 5 |

## What's next

In the next file, you'll set up your development environment and confirm the .NET SDK is installed correctly.

**Next → `02-setup-environment.md`**
