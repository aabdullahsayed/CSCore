# 7. Unit and Integration Testing

## Test project setup

```bash
dotnet new xunit -n MyFirstApi.Tests
cd MyFirstApi.Tests
dotnet add reference ../MyFirstApi/MyFirstApi.csproj
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

## Unit testing a controller (with mocked dependencies)

```csharp
public class BooksControllerTests
{
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Book?)null);
        var controller = new BooksController(mockRepo.Object);

        // Act
        var result = await controller.GetById(99);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_ReturnsOk_WithCorrectBook()
    {
        var book = new Book { Id = 1, Title = "Clean Code" };
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        var controller = new BooksController(mockRepo.Object);

        var result = await controller.GetById(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(book);
    }
}
```

**AAA pattern**: Arrange (set up inputs/mocks), Act (call the method under test), Assert (verify the outcome). Keep each test focused on a single behavior.

## Unit testing services/business logic

```csharp
public class BookServiceTests
{
    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenPriceIsNegative()
    {
        var mockRepo = new Mock<IBookRepository>();
        var service = new BookService(mockRepo.Object);

        var act = () => service.CreateAsync(new CreateBookDto { Title = "X", Price = -5 });

        await act.Should().ThrowAsync<ValidationException>();
    }
}
```

## Integration testing with `WebApplicationFactory`

Integration tests spin up the real app (in-memory) with a real (or test) database, and exercise it through actual HTTP calls — catching wiring issues unit tests can't.

```csharp
public class BooksApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BooksApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace real DbContext with an in-memory one for tests
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessAndCorrectContentType()
    {
        var response = await _client.GetAsync("/api/books");

        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType!.ToString().Should().Be("application/json; charset=utf-8");
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithLocationHeader()
    {
        var dto = new CreateBookDto { Title = "New Book", Author = "Author", Price = 19.99m };

        var response = await _client.PostAsJsonAsync("/api/books", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }
}
```

`Program.cs` needs a `public partial class Program { }` at the bottom (top-level statement templates already generate this) so `WebApplicationFactory<Program>` can reference it.

## Test data builders (reduce duplication)

```csharp
public class BookBuilder
{
    private readonly Book _book = new() { Title = "Default Title", Author = "Default Author", Price = 9.99m };

    public BookBuilder WithTitle(string title) { _book.Title = title; return this; }
    public BookBuilder WithPrice(decimal price) { _book.Price = price; return this; }
    public Book Build() => _book;
}

// usage: var book = new BookBuilder().WithTitle("Dune").WithPrice(25.00m).Build();
```

## What to test at each layer

| Layer | Test type | Focus |
|---|---|---|
| Controllers | Unit | Correct status codes, correct delegation to services |
| Services / business logic | Unit | Validation rules, calculations, edge cases |
| Repositories / EF Core queries | Integration | Real query behavior against a test DB |
| Full HTTP pipeline | Integration | Routing, middleware, auth, serialization all wired correctly |

## Practice task

Write unit tests covering every branch of your `BooksController` (found, not found, validation failure) and one integration test that creates a book via `POST` and then retrieves it via `GET`.

## Advanced section checklist

- [ ] I can secure endpoints with JWT authentication
- [ ] I can implement role- and policy-based authorization
- [ ] I understand API versioning strategies
- [ ] I can document and secure my API with Swagger
- [ ] I have centralized exception handling returning `ProblemDetails`
- [ ] I understand when and how to cache (in-memory vs. distributed)
- [ ] I can write both unit and integration tests

**Next → `08-repository-and-unit-of-work.md`**
