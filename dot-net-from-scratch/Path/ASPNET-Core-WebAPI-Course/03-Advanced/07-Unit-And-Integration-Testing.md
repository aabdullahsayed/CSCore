# 7. Unit and Integration Testing

## Testing pyramid recap

```
        ▲
       / \        E2E (few) — full system, slow, brittle
      /---\
     /     \      Integration (some) — real DB/HTTP pipeline, moderate speed
    /-------\
   /         \    Unit (many) — isolated logic, fast, cheap
  /-----------\
```

## Setting up the test project

```bash
dotnet new xunit -n BookStore.Api.Tests
cd BookStore.Api.Tests
dotnet add reference ../BookStore.Api/BookStore.Api.csproj
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

## Unit testing a service (with mocked dependencies)

```csharp
using Xunit;
using Moq;
using FluentAssertions;

public class BookServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Book { Id = 1, Title = "Dune", Author = "Frank Herbert" });

        var service = new BookService(mockRepo.Object, Mock.Of<IMapper>());

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Dune");
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenBookDoesNotExist()
    {
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Book?)null);

        var service = new BookService(mockRepo.Object, Mock.Of<IMapper>());

        var act = async () => await service.GetByIdAsync(999);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
```

## Anatomy of a good unit test: Arrange / Act / Assert

- **Arrange** — set up inputs and mocks
- **Act** — call the method under test
- **Assert** — verify the outcome

Keep each test focused on **one** behavior; name tests descriptively: `MethodName_ExpectedBehavior_WhenCondition`.

## Parameterized tests

```csharp
[Theory]
[InlineData(-1)]
[InlineData(0)]
public async Task CreateAsync_ThrowsValidationException_WhenPriceIsInvalid(decimal price)
{
    var dto = new CreateBookDto { Title = "Test", Author = "Test", Price = price };
    var service = new BookService(Mock.Of<IBookRepository>(), Mock.Of<IMapper>());

    var act = async () => await service.CreateAsync(dto);

    await act.Should().ThrowAsync<ValidationException>();
}
```

## Integration testing with WebApplicationFactory

Integration tests spin up your entire app in-memory (real middleware pipeline, real routing) and hit it with real HTTP requests — but typically against a test database.

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
                // Replace the real DbContext with an in-memory one for testing
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor is not null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetAllBooks_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/books");

        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateBook_ReturnsCreated_WithLocationHeader()
    {
        var newBook = new { title = "New Book", author = "Author", price = 12.99m };
        var response = await _client.PostAsJsonAsync("/api/books", newBook);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }
}
```

> Note: `Program.cs` needs a `public partial class Program { }` line at the bottom (or use the top-level statements as-is in .NET 6+, which auto-generates this) so `WebApplicationFactory<Program>` can reference it.

## Testing with a real (but disposable) database — Testcontainers

For higher-fidelity integration tests than the EF Core In-Memory provider (which doesn't enforce real relational constraints), use **Testcontainers** to spin up a real, disposable SQL Server/PostgreSQL Docker container per test run:

```bash
dotnet add package Testcontainers.PostgreSql
```

This is the gold standard for integration tests in serious production codebases — the EF Core In-Memory provider is fine for learning/simple cases but can hide real SQL-specific bugs.

## What to test at each layer

| Layer | Test type | Focus |
|---|---|---|
| Services / business logic | Unit tests, mocked repo | Business rules, edge cases, error paths |
| Repositories | Integration tests, real/test DB | Actual query correctness |
| Controllers / full pipeline | Integration tests, `WebApplicationFactory` | Routing, status codes, serialization, auth |

## Running tests

```bash
dotnet test
dotnet test --filter "FullyQualifiedName~BookServiceTests"
dotnet test /p:CollectCoverage=true   # with coverlet.collector installed
```

---
**Next:** `08-Docker-And-Deployment.md`
