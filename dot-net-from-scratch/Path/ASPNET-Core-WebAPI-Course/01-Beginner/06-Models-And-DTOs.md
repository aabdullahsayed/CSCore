# 6. Models and DTOs

## What's a DTO?

**DTO** = Data Transfer Object. It's a plain class shaped specifically for what a client sends or receives — separate from your internal domain/database model (often called an **Entity**).

## Why not just return your Entity directly?

Problems with exposing entities directly:

1. **Over-exposure** — your `User` entity might have a `PasswordHash` field you never want to send to a client.
2. **Tight coupling** — if you change a database column, your API contract breaks for every client.
3. **Over-fetching** — clients get fields they don't need (bigger payloads, wasted bandwidth).
4. **Different shapes for different operations** — creating a book doesn't need an `Id` (server generates it); reading a book might need a computed `DiscountedPrice` the database doesn't store.

## Example: Entity vs DTOs

**Entity** (maps to a database table, used internally):

```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }         // soft-delete flag — internal only!
}
```

**DTOs** (what the API actually exposes):

```csharp
// What a client sends to CREATE a book — no Id, no timestamps, no IsDeleted
public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// What a client sends to UPDATE a book
public class UpdateBookDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// What the API returns when READING a book
public class BookResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

## Using DTOs in a controller

```csharp
[HttpPost]
public ActionResult<BookResponseDto> Create([FromBody] CreateBookDto dto)
{
    var book = new Book
    {
        Id = _books.Max(b => b.Id) + 1,
        Title = dto.Title,
        Author = dto.Author,
        Price = dto.Price,
        CreatedAt = DateTime.UtcNow
    };
    _books.Add(book);

    var response = new BookResponseDto
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Price = book.Price
    };

    return CreatedAtAction(nameof(GetById), new { id = book.Id }, response);
}
```

This manual mapping (Entity → DTO, DTO → Entity) gets repetitive fast — in `02-Intermediate/08-AutoMapper-And-Clean-Layering.md` you'll learn to automate it with **AutoMapper**.

## Naming conventions used in this course

| Suffix | Meaning |
|---|---|
| `...Dto` | generic transfer object |
| `Create...Dto` | shape for POST requests |
| `Update...Dto` | shape for PUT/PATCH requests |
| `...ResponseDto` or just `...Dto` | shape returned to the client |
| Entity (no suffix) | maps 1:1 to a database table |

## Records vs classes for DTOs

C# `record` types are a great fit for DTOs because they're immutable-by-default and get free value equality:

```csharp
public record CreateBookDto(string Title, string Author, decimal Price);
```

Both styles are used in real projects — classes are more common when you need mutable properties (e.g., binding form data); records are popular for pure request/response contracts.

---

## 🎯 Practice Task (end of Beginner stage)

Build a small **"Task Manager" API** from scratch:
- `TaskItem` entity: `Id`, `Title`, `IsComplete`, `DueDate`
- `CreateTaskDto`, `UpdateTaskDto`, `TaskResponseDto`
- Full CRUD controller (`GET` all, `GET` by id, `POST`, `PUT`, `DELETE`)
- Test every endpoint with curl or Swagger UI
- Bonus: add a `GET /api/tasks?isComplete=true` filter using `[FromQuery]`

Once this works end-to-end with an in-memory list, move to the Intermediate section to swap the in-memory list for a real database.

---
**Next stage:** `../02-Intermediate/01-Dependency-Injection.md`
