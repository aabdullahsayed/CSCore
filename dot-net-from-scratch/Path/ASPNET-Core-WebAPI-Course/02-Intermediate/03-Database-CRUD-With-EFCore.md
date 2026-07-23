# 3. Database CRUD with EF Core

## The repository, backed by a real database

```csharp
using Microsoft.EntityFrameworkCore;
using BookStore.Api.Data;
using BookStore.Api.Models;

namespace BookStore.Api.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Book>> GetAllAsync() =>
        await _context.Books.AsNoTracking().ToListAsync();

    public async Task<Book?> GetByIdAsync(int id) =>
        await _context.Books.FindAsync(id);

    public async Task<Book> CreateAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        var existing = await _context.Books.FindAsync(book.Id);
        if (existing is null) return false;

        existing.Title = book.Title;
        existing.Author = book.Author;
        existing.Price = book.Price;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}
```

Register it:
```csharp
builder.Services.AddScoped<IBookRepository, BookRepository>();
```

## Key EF Core concepts used above

- **`AsNoTracking()`** — for read-only queries, skips EF Core's change-tracking overhead. Use it for every `GET` that won't be updated in the same request.
- **`FindAsync(id)`** — looks up by primary key; checks the in-memory tracked entities first before querying the database.
- **`SaveChangesAsync()`** — nothing is written to the database until you call this. EF Core batches your changes.

## LINQ queries you'll use constantly

```csharp
// Filtering
var cheapBooks = await _context.Books
    .Where(b => b.Price < 15)
    .ToListAsync();

// Sorting
var sorted = await _context.Books
    .OrderBy(b => b.Title)
    .ToListAsync();

// Paging
var page = await _context.Books
    .OrderBy(b => b.Id)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// Searching
var results = await _context.Books
    .Where(b => b.Title.Contains(searchTerm))
    .ToListAsync();

// Projecting directly to a DTO (avoids loading unused columns)
var summaries = await _context.Books
    .Select(b => new BookResponseDto { Id = b.Id, Title = b.Title })
    .ToListAsync();

// Aggregates
var count = await _context.Books.CountAsync();
var totalValue = await _context.Books.SumAsync(b => b.Price);
```

## Relationships (one-to-many example)

```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Book> Books { get; set; } = new();
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }          // foreign key
    public Author? Author { get; set; }        // navigation property
}
```

Loading related data — **eager loading** with `Include`:

```csharp
var books = await _context.Books
    .Include(b => b.Author)
    .ToListAsync();
```

Without `Include`, `book.Author` would be `null` (EF Core doesn't auto-load related data — this avoids accidental N+1 query explosions).

## Updating the controller to use async all the way through

```csharp
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _repo;
    public BooksController(IBookRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> GetById(int id)
    {
        var book = await _repo.GetByIdAsync(id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> Create(Book book)
    {
        var created = await _repo.CreateAsync(book);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Book book)
    {
        if (id != book.Id) return BadRequest("Id mismatch");
        var updated = await _repo.UpdateAsync(book);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await _repo.DeleteAsync(id) ? NoContent() : NotFound();
}
```

---
**Next:** `04-Model-Validation.md`
