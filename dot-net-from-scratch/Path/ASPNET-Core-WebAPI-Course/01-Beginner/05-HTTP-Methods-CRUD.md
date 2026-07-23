# 5. HTTP Methods & Building a CRUD API (In-Memory)

Before touching a real database, let's build a complete CRUD (Create, Read, Update, Delete) API using an in-memory list. This cements the HTTP verb → action mapping before we add EF Core complexity in the Intermediate section.

## The HTTP verbs, precisely

| Verb | Purpose | Idempotent? | Has body? |
|---|---|---|---|
| GET | Retrieve a resource | Yes | No |
| POST | Create a new resource | No | Yes |
| PUT | Replace a resource entirely | Yes | Yes |
| PATCH | Partially update a resource | No* | Yes |
| DELETE | Remove a resource | Yes | No |

*Idempotent: calling it multiple times has the same effect as calling it once. `POST` is not idempotent (calling it twice creates two resources); `PUT`/`DELETE` are.

## The model

```csharp
namespace BookStore.Api.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

## The full controller

```csharp
using Microsoft.AspNetCore.Mvc;
using BookStore.Api.Models;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    // Static list simulates a database for now (resets on app restart).
    private static readonly List<Book> _books = new()
    {
        new Book { Id = 1, Title = "Dune", Author = "Frank Herbert", Price = 15.99m },
        new Book { Id = 2, Title = "1984", Author = "George Orwell", Price = 9.99m }
    };

    // GET api/books
    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetAll() => Ok(_books);

    // GET api/books/5
    [HttpGet("{id:int}")]
    public ActionResult<Book> GetById(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book is null) return NotFound(new { message = $"Book {id} not found." });
        return Ok(book);
    }

    // POST api/books
    [HttpPost]
    public ActionResult<Book> Create([FromBody] Book newBook)
    {
        newBook.Id = _books.Max(b => b.Id) + 1;
        _books.Add(newBook);

        // 201 Created with a Location header pointing to GET api/books/{id}
        return CreatedAtAction(nameof(GetById), new { id = newBook.Id }, newBook);
    }

    // PUT api/books/5  (full replace)
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Book updatedBook)
    {
        var existing = _books.FirstOrDefault(b => b.Id == id);
        if (existing is null) return NotFound();

        existing.Title = updatedBook.Title;
        existing.Author = updatedBook.Author;
        existing.Price = updatedBook.Price;

        return NoContent();   // 204 — success, nothing to return
    }

    // DELETE api/books/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book is null) return NotFound();

        _books.Remove(book);
        return NoContent();
    }
}
```

## Testing it with curl

```bash
# List all
curl https://localhost:7031/api/books

# Get one
curl https://localhost:7031/api/books/1

# Create
curl -X POST https://localhost:7031/api/books \
  -H "Content-Type: application/json" \
  -d '{"title":"Brave New World","author":"Aldous Huxley","price":11.50}'

# Update
curl -X PUT https://localhost:7031/api/books/1 \
  -H "Content-Type: application/json" \
  -d '{"title":"Dune (Updated)","author":"Frank Herbert","price":17.99}'

# Delete
curl -X DELETE https://localhost:7031/api/books/1
```

## Status code cheat sheet for CRUD

| Action | Success | Not found | Bad input |
|---|---|---|---|
| GET all | 200 | — | — |
| GET by id | 200 | 404 | 400 |
| POST | 201 | — | 400 |
| PUT | 204 or 200 | 404 | 400 |
| DELETE | 204 | 404 | — |

---
**Next:** `06-Models-And-DTOs.md` — why you shouldn't expose your database models directly.
