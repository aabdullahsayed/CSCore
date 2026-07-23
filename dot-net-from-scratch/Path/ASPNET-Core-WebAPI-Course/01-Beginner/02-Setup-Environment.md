# 2. Setting Up Your Environment

## 1. Install the .NET SDK

Download the **.NET 8 SDK** (LTS) from the official source:
https://dotnet.microsoft.com/download

Verify installation:

```bash
dotnet --version
# should print something like 8.0.xxx
```

```bash
dotnet --list-sdks
```

## 2. Choose an editor / IDE

Any of these work well:

| Editor | Notes |
|---|---|
| **Visual Studio 2022** (Windows) | Full IDE, best debugging experience, free Community edition |
| **Visual Studio Code** (any OS) | Lightweight, install the **C# Dev Kit** extension |
| **JetBrains Rider** (any OS) | Paid, excellent refactoring tools |

For this course, examples use the `dotnet` CLI, so any editor works.

## 3. Useful CLI commands you'll use constantly

```bash
dotnet new webapi -n MyApi        # scaffold a new Web API project
dotnet run                        # run the project
dotnet watch run                  # run with hot-reload on file save
dotnet build                      # compile without running
dotnet add package <PackageName>  # install a NuGet package
dotnet restore                    # restore dependencies
dotnet ef migrations add <Name>   # (after installing EF tools) add a migration
dotnet test                       # run tests
```

## 4. Install the EF Core CLI tool (needed later, install now to save time)

```bash
dotnet tool install --global dotnet-ef
dotnet ef --version
```

## 5. Get a way to test your API

You need a tool to send HTTP requests manually while developing:

- **Swagger UI** — built into the template, no install needed (we'll use this by default).
- **Postman** — https://www.postman.com/downloads/ (popular, full-featured).
- **`.http` files** in VS Code / Rider — lightweight, version-controllable request files.
- **curl** — always available on the command line.

Example curl request you'll be able to run once your first project is up:

```bash
curl https://localhost:5001/weatherforecast
```

## 6. Optional but recommended: install a database tool

We'll use SQL Server LocalDB (Windows) or SQLite/PostgreSQL (cross-platform) later in the Intermediate section. For now, no action needed — we'll install per-project.

---
**Next:** `03-First-WebAPI-Project.md` — scaffold and run your first API.
