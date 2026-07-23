# 3. Model Validation and DTOs

## Why DTOs (Data Transfer Objects)?

Never expose your EF Core entities directly in requests/responses. Use separate **DTO classes** to:

- Control exactly which fields clients can send/see
- Avoid over-posting attacks (client setting fields like `IsAdmin` they shouldn't touch)
- Decouple your public API contract from your internal database schema

```csharp
// What the client sends when creating a book
public class CreateBookDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    [Range(0.01, 10000)]
    public decimal Price { get; set; }
}

// What the API returns
public class BookResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

## Data annotations reference

| Attribute | Purpose |
|---|---|
| `[Required]` | Field must not be null/empty |
| `[MaxLength(n)]` / `[MinLength(n)]` | String/collection length |
| `[Range(min, max)]` | Numeric range |
| `[EmailAddress]` | Valid email format |
| `[RegularExpression(pattern)]` | Custom pattern match |
| `[Compare("OtherProperty")]` | Must match another property |
| `[StringLength(n, MinimumLength = m)]` | Combined min/max length |

## Automatic validation with `[ApiController]`

Because `[ApiController]` is applied, ASP.NET Core automatically returns `400 Bad Request` with a `ValidationProblemDetails` body when a DTO fails validation — **you don't need to check `ModelState.IsValid` manually** in most cases:

```csharp
[HttpPost]
public async Task<ActionResult<BookResponseDto>> Create(CreateBookDto dto)
{
    // If dto is invalid, ASP.NET Core already returned 400 before this method runs.
    var book = new Book { Title = dto.Title, Author = dto.Author, Price = dto.Price };
    await _repository.AddAsync(book);

    var response = new BookResponseDto { Id = book.Id, Title = book.Title, Author = book.Author, Price = book.Price };
    return CreatedAtAction(nameof(GetById), new { id = book.Id }, response);
}
```

Example error response the client receives automatically:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["The Title field is required."],
    "Price": ["The field Price must be between 0.01 and 10000."]
  }
}
```

## Custom validation logic

For rules that span multiple properties, implement `IValidatableObject`:

```csharp
public class DateRangeDto : IValidatableObject
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (EndDate < StartDate)
            yield return new ValidationResult("EndDate must be after StartDate.", new[] { nameof(EndDate) });
    }
}
```

## Mapping between DTOs and entities

For small projects, map manually (as shown above). For larger projects, use **AutoMapper** or **Mapster** to reduce boilerplate:

```bash
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookResponseDto>();
        CreateMap<CreateBookDto, Book>();
    }
}

// Program.cs
builder.Services.AddAutoMapper(typeof(MappingProfile));
```

## Practice task

Add `CreateBookDto` and `UpdateBookDto` to your Books API, wire up validation attributes, and confirm a `POST` with an empty `Title` returns a `400` with a descriptive error message.

**Next → `04-middleware-pipeline.md`**
