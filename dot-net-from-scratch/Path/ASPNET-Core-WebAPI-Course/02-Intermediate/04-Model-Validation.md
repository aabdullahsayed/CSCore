# 4. Model Validation

## Why validate?

Never trust client input. Validation protects data integrity and gives clients clear, actionable error messages instead of crashes or corrupted data.

## Data Annotations (declarative validation)

```csharp
using System.ComponentModel.DataAnnotations;

public class CreateBookDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Author { get; set; } = string.Empty;

    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000.")]
    public decimal Price { get; set; }

    [EmailAddress]
    public string? ContactEmail { get; set; }

    [RegularExpression(@"^\d{3}-\d{10}$", ErrorMessage = "ISBN must match ###-##########.")]
    public string? Isbn { get; set; }
}
```

Common attributes: `[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`, `[Phone]`, `[Url]`, `[RegularExpression]`, `[MinLength]`/`[MaxLength]`, `[Compare]` (e.g., password confirmation).

## Automatic validation with [ApiController]

Because your controller has `[ApiController]`, ASP.NET Core **automatically** checks `ModelState` and returns `400 Bad Request` with error details if validation fails — you don't even need an `if` check:

```csharp
[HttpPost]
public async Task<ActionResult<Book>> Create(CreateBookDto dto)
{
    // If dto fails validation, this line never runs —
    // the framework already returned 400 automatically.
    var book = new Book { Title = dto.Title, Author = dto.Author, Price = dto.Price };
    ...
}
```

Example automatic error response:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["Title is required."],
    "Price": ["Price must be between 0.01 and 10000."]
  }
}
```

## Custom validation attributes

```csharp
public class NotPastDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is DateTime date && date < DateTime.UtcNow.Date)
            return new ValidationResult("Date cannot be in the past.");

        return ValidationResult.Success;
    }
}

public class CreateEventDto
{
    [NotPastDate]
    public DateTime EventDate { get; set; }
}
```

## IValidatableObject for cross-field validation

```csharp
public class CreateDiscountDto : IValidatableObject
{
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (DiscountedPrice >= OriginalPrice)
        {
            yield return new ValidationResult(
                "Discounted price must be lower than the original price.",
                new[] { nameof(DiscountedPrice) });
        }
    }
}
```

## FluentValidation — a popular alternative (recommended for larger projects)

```bash
dotnet add package FluentValidation.AspNetCore
```

```csharp
using FluentValidation;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).InclusiveBetween(0.01m, 10000m);
    }
}
```

Register:
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookDtoValidator>();
```

**Why prefer FluentValidation on bigger projects?** Validation logic lives outside the DTO (better separation of concerns), supports complex conditional rules cleanly, and is easier to unit test in isolation.

## Manually returning validation errors (when NOT using [ApiController] auto-behavior)

```csharp
if (!ModelState.IsValid)
    return BadRequest(ModelState);
```

---
**Next:** `05-Middleware-Pipeline.md`
