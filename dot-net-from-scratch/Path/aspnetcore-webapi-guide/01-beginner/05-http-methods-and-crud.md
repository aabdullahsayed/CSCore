# 5. HTTP Methods and a Full CRUD Example

## HTTP status codes you must know

| Code | Meaning | When to use |
|---|---|---|
| 200 OK | Success | GET, successful PUT/PATCH |
| 201 Created | Resource created | Successful POST |
| 204 No Content | Success, nothing to return | Successful DELETE |
| 400 Bad Request | Client sent invalid data | Failed validation |
| 401 Unauthorized | Not authenticated | Missing/invalid token |
| 403 Forbidden | Authenticated but not allowed | Insufficient permissions |
| 404 Not Found | Resource doesn't exist | Bad id |
| 409 Conflict | State conflict | Duplicate resource |
| 500 Internal Server Error | Unhandled exception | Bugs, unexpected errors |

## A complete in-memory CRUD controller

```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    // Static list for demo purposes only — replace with a database later.
    private static readonly List<Book> Books = new()
    {
        new Book { Id = 1, Title = "Clean Code", Author = "Robert C. Martin", Price = 35.99m },
        new Book { Id = 2, Title = "The Pragmatic Programmer", Author = "Hunt & Thomas", Price = 42.50m }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetAll() => Ok(Books);

    [HttpGet("{id:int}")]
    public ActionResult<Book> GetById(int id)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public ActionResult<Book> Create(Book newBook)
    {
        newBook.Id = Books.Max(b => b.Id) + 1;
        Books.Add(newBook);
        return CreatedAtAction(nameof(GetById), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Book updated)
    {
        var existing = Books.FirstOrDefault(b => b.Id == id);
        if (existing is null) return NotFound();

        existing.Title = updated.Title;
        existing.Author = updated.Author;
        existing.Price = updated.Price;
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = Books.FirstOrDefault(b => b.Id == id);
        if (existing is null) return NotFound();

        Books.Remove(existing);
        return NoContent();
    }
}
```

## Testing with an `.http` file

Create `requests.http` in the project root:

```http
### Get all books
GET https://localhost:7123/api/books

### Get one book
GET https://localhost:7123/api/books/1

### Create a book
POST https://localhost:7123/api/books
Content-Type: application/json

{
  "title": "Domain-Driven Design",
  "author": "Eric Evans",
  "price": 49.99
}

### Update a book
PUT https://localhost:7123/api/books/1
Content-Type: application/json

{
  "title": "Clean Code (2nd ed.)",
  "author": "Robert C. Martin",
  "price": 39.99
}

### Delete a book
DELETE https://localhost:7123/api/books/2
```

## Key takeaway

At this point you can build a fully working, if simplistic, CRUD API. What's missing — and what the **Intermediate** section covers — is: persisting data to a real database, validating input properly, injecting services instead of hardcoding static lists, and structuring the app for growth.

## Beginner section checklist

- [ ] I understand what REST is and why status codes matter
- [ ] I can create a new Web API project from the CLI
- [ ] I understand `Program.cs`'s two phases (services vs. pipeline)
- [ ] I can write a controller with full CRUD and attribute routing
- [ ] I can test endpoints with `.http` files or Postman

**Next → `../02-intermediate/01-dependency-injection.md`**
