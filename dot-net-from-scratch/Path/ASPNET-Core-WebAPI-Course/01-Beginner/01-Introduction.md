# 1. Introduction to ASP.NET Core Web API

## What is a Web API?

A **Web API** is a server application that exposes endpoints over HTTP so that other applications (mobile apps, front-end frameworks like React/Angular, other services, etc.) can send requests and receive data — usually in JSON format — instead of full HTML pages.

```
Client (React app, mobile app, Postman)
        │  HTTP Request (GET /api/products)
        ▼
   ASP.NET Core Web API
        │  JSON Response
        ▼
Client renders / consumes the data
```

## What is ASP.NET Core?

ASP.NET Core is a free, open-source, cross-platform framework (built by Microsoft) for building web apps and APIs. It runs on Windows, macOS, and Linux, and is built on top of **.NET** (the runtime and set of libraries).

Key traits:
- **Cross-platform** — develop and deploy on any OS.
- **High performance** — one of the fastest mainstream web frameworks available.
- **Open source** — code is on GitHub, community-driven.
- **Modular** — you only include the pieces (NuGet packages) you actually need.
- **Built-in dependency injection, logging, configuration** — no need for third-party glue out of the box.

## Web API vs MVC vs Razor Pages

| Type | Returns | Use case |
|---|---|---|
| MVC | HTML views | Traditional server-rendered websites |
| Razor Pages | HTML pages | Page-focused websites |
| **Web API** | JSON / XML data | Backend for SPAs, mobile apps, microservices |

This course focuses purely on **Web API** — no HTML views, just data endpoints.

## Core building blocks you'll learn

1. **Controllers** — classes that handle incoming requests.
2. **Routing** — mapping URLs to controller actions.
3. **Models/DTOs** — shapes of the data you send and receive.
4. **Middleware** — pipeline components that process every request (logging, auth, error handling).
5. **Dependency Injection (DI)** — how ASP.NET Core wires services together.
6. **Entity Framework Core (EF Core)** — talking to a database using C# instead of raw SQL.
7. **Authentication/Authorization** — who can call what.

## A note on versions

This course uses **.NET 8** (a Long-Term Support release) and the **minimal hosting model** (the modern `Program.cs` style introduced in .NET 6+, without `Startup.cs`). Concepts carry over cleanly to .NET 9/10 as well.

## What "REST" means (you'll hear this a lot)

REST (Representational State Transfer) is a set of conventions for designing APIs:
- Resources are nouns, not verbs: `/api/products`, not `/api/getProducts`
- HTTP methods express intent: `GET` (read), `POST` (create), `PUT`/`PATCH` (update), `DELETE` (remove)
- Responses use standard HTTP status codes: `200 OK`, `201 Created`, `404 Not Found`, `400 Bad Request`, etc.
- The API is stateless — each request contains everything needed to process it (no server-side session state between requests).

We'll apply these conventions from the very first project.

---
**Next:** `02-Setup-Environment.md` — installing the .NET SDK and your editor.
