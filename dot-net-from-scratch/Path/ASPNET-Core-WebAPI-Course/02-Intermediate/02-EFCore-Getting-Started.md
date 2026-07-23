# 2. Entity Framework Core — Getting Started

## What is EF Core?

EF Core is an **ORM** (Object-Relational Mapper): it lets you work with a database using C# classes and LINQ instead of writing raw SQL by hand.

```
C# Class (Book)  ⇄  EF Core  ⇄  Database Table (Books)
```

## Install the packages

For SQL Server:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

For SQLite (simplest for learning — no server needed, just a file):
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

This course uses **SQLite** for simplicity; swapping to SQL Server/PostgreSQL later only changes the connection string and the `Use...()` call.

## Define your entity

```csharp
namespace BookStore.Api.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

## Create the DbContext

The `DbContext` is your session with the database — it tracks entities and translates LINQ to SQL.

```csharp
using Microsoft.EntityFrameworkCore;
using BookStore.Api.Models;

namespace BookStore.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Price).HasColumnType("decimal(10,2)");
        });
    }
}
```

## Register the DbContext in Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using BookStore.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
var app = builder.Build();
```

`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=bookstore.db"
  }
}
```

> `AddDbContext` registers the context as **Scoped** by default — one instance per HTTP request, which is exactly right (see the DI lesson).

## Migrations — creating your database schema from code

Migrations track changes to your model over time and apply them to the database.

```bash
# Create the first migration (compares your model to an empty database)
dotnet ef migrations add InitialCreate

# Apply it — this actually creates the database/tables
dotnet ef database update
```

This generates a `Migrations/` folder with C# files describing the schema, plus a `Migrations/AppDbContextModelSnapshot.cs` tracking the current state.

Whenever you change your entity classes later:

```bash
dotnet ef migrations add AddPublishedYearToBooks
dotnet ef database update
```

## Common EF Core CLI commands

```bash
dotnet ef migrations add <Name>       # create a new migration
dotnet ef migrations remove           # undo the last unapplied migration
dotnet ef database update             # apply pending migrations
dotnet ef database update <Migration> # roll back to a specific migration
dotnet ef migrations list             # list all migrations
dotnet ef dbcontext info              # show DbContext info
```

## Seeding initial data (optional, useful for demos)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Book>().HasData(
        new Book { Id = 1, Title = "Dune", Author = "Frank Herbert", Price = 15.99m },
        new Book { Id = 2, Title = "1984", Author = "George Orwell", Price = 9.99m }
    );
}
```
Then create and apply a migration to push the seed data.

---
**Next:** `03-Database-CRUD-With-EFCore.md` — wiring the repository to real EF Core queries.
