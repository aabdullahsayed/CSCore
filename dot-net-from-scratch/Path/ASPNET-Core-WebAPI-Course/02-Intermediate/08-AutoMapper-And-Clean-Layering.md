# 8. AutoMapper and Clean Layering

## Recap: the layers of a well-structured Web API

```
Controller        → handles HTTP concerns only (routing, status codes)
   ↓
Service (business logic) → validation rules, orchestration, calculations
   ↓
Repository         → data access (EF Core queries)
   ↓
DbContext / Database
```

DTOs flow in from the Controller down to the Service; Entities flow between Service and Repository. This is called a **layered (n-tier) architecture**.

## Installing AutoMapper

```bash
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

## Defining mapping profiles

```csharp
using AutoMapper;
using BookStore.Api.Models;
using BookStore.Api.Dtos;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookResponseDto>();
        CreateMap<CreateBookDto, Book>();
        CreateMap<UpdateBookDto, Book>();
    }
}
```

Register it:
```csharp
builder.Services.AddAutoMapper(typeof(BookProfile));
```

## Using AutoMapper in a service

```csharp
public class BookService : IBookService
{
    private readonly IBookRepository _repo;
    private readonly IMapper _mapper;

    public BookService(IBookRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookResponseDto>> GetAllAsync()
    {
        var books = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<BookResponseDto>>(books);
    }

    public async Task<BookResponseDto> CreateAsync(CreateBookDto dto)
    {
        var book = _mapper.Map<Book>(dto);
        var created = await _repo.CreateAsync(book);
        return _mapper.Map<BookResponseDto>(created);
    }
}
```

## Custom mapping rules

```csharp
CreateMap<Book, BookResponseDto>()
    .ForMember(dest => dest.DisplayName,
        opt => opt.MapFrom(src => $"{src.Title} by {src.Author}"));

CreateMap<CreateBookDto, Book>()
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
    .ForMember(dest => dest.Id, opt => opt.Ignore());   // never trust client-supplied Id
```

## Should you always use AutoMapper?

Trade-offs:

| Manual mapping | AutoMapper |
|---|---|
| More boilerplate | Less boilerplate |
| Easy to debug (just C# code) | "Magic" — harder to trace, needs `.ForMember` for anything non-trivial |
| No extra dependency | Extra NuGet dependency + startup cost |
| Great for small projects | Shines on large projects with many DTOs |

Many teams now prefer **manual mapping via extension methods** or C# `record` mapping constructors for simplicity and easier debugging:

```csharp
public static class BookMappingExtensions
{
    public static BookResponseDto ToDto(this Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Price = book.Price
    };

    public static Book ToEntity(this CreateBookDto dto) => new()
    {
        Title = dto.Title,
        Author = dto.Author,
        Price = dto.Price
    };
}

// usage: var dto = book.ToDto();
```

Both approaches are valid — pick one and be consistent across the project.

## The Service layer — why bother?

Putting business logic in a Service (rather than directly in the controller) means:
- Controllers stay thin (only HTTP concerns).
- Business logic is testable without spinning up HTTP infrastructure.
- Logic is reusable (e.g., called from a background job, not just a controller).

```csharp
public interface IBookService
{
    Task<IEnumerable<BookResponseDto>> GetAllAsync();
    Task<BookResponseDto?> GetByIdAsync(int id);
    Task<BookResponseDto> CreateAsync(CreateBookDto dto);
    Task<bool> UpdateAsync(int id, UpdateBookDto dto);
    Task<bool> DeleteAsync(int id);
}
```

```csharp
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;
    public BooksController(IBookService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<BookResponseDto>> Create(CreateBookDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    // ...
}
```

---

## 🎯 Practice Task (end of Intermediate stage)

Extend your Task Manager API from the Beginner stage:
1. Add EF Core with SQLite, migrate `TaskItem` into a real table.
2. Add a `Category` entity with a one-to-many relationship to `TaskItem`.
3. Add DataAnnotation validation to your DTOs (`Title` required, max length 100).
4. Introduce a `TaskService` layer between controller and repository.
5. Add a logging middleware that logs method, path, and response time for every request.

---
**Next stage:** `../03-Advanced/01-Authentication-JWT.md`
