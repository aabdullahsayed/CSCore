# 2. Setting Up Your Environment

## 1. Install the .NET SDK

Download the latest LTS SDK from https://dotnet.microsoft.com/download

Verify installation:

```bash
dotnet --version
dotnet --list-sdks
```

## 2. Choose an editor

- **Visual Studio 2022** (Windows) — richest tooling, best debugger, built-in scaffolding.
- **VS Code** (cross-platform) — install the **C# Dev Kit** extension.
- **JetBrains Rider** (cross-platform) — excellent refactoring tools.

## 3. Useful global tools

```bash
# Scaffold and manage EF Core migrations later
dotnet tool install --global dotnet-ef

# Watch and auto-reload on file changes
dotnet watch --help
```

## 4. Understand the `dotnet` CLI

| Command | Purpose |
|---|---|
| `dotnet new webapi -n MyApi` | Create a new Web API project |
| `dotnet build` | Compile the project |
| `dotnet run` | Run the project |
| `dotnet watch run` | Run with hot reload |
| `dotnet add package <name>` | Add a NuGet package |
| `dotnet restore` | Restore dependencies |
| `dotnet test` | Run tests |

## 5. Project templates

```bash
dotnet new webapi -n MyApi --use-controllers
```

- `--use-controllers` gives you traditional controller classes (recommended while learning).
- Omitting it (in newer templates) generates a **minimal API** style `Program.cs`. You'll learn both — minimal APIs are covered later.

## 6. Recommended folder layout for solutions

As projects grow you'll typically split into multiple projects inside one solution:

```
MySolution.sln
├── src/
│   ├── MyApi.Api/            # controllers, Program.cs
│   ├── MyApi.Application/    # business logic, services
│   ├── MyApi.Domain/         # entities, interfaces
│   └── MyApi.Infrastructure/ # EF Core, external services
└── tests/
    ├── MyApi.UnitTests/
    └── MyApi.IntegrationTests/
```

This layered structure is explained fully in `04-expert/01-clean-architecture.md`. For now, a single project is fine.

## 7. Testing your API

Two easy ways to call your endpoints while developing:

- **`.http` files** — ASP.NET Core project templates generate a `MyApi.http` file. VS Code and Rider can execute requests directly from it.
- **Postman / Insomnia** — GUI tools for sending requests and inspecting responses.
- **Swagger UI** — auto-generated interactive docs, covered in `03-advanced/04-swagger-openapi.md`.

**Next → `03-first-web-api.md`**
