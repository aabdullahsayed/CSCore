# 2. Authorization — Roles, Claims, and Policies

## The simplest form: [Authorize]

```csharp
[Authorize]                    // any authenticated user
[HttpGet]
public IActionResult Get() => Ok();

[AllowAnonymous]               // explicitly public, even inside a locked-down controller
[HttpGet("public-info")]
public IActionResult PublicInfo() => Ok();
```

You can apply `[Authorize]` at the controller level to protect every action by default, then use `[AllowAnonymous]` to open specific ones:

```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll() => Ok();   // public

    [HttpPost]
    public IActionResult Create() => Ok();   // requires auth
}
```

## Role-based authorization

Roles are added as claims when the JWT is generated (see previous file: `new Claim(ClaimTypes.Role, r)`).

```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public IActionResult Delete(int id) => NoContent();

[Authorize(Roles = "Admin,Manager")]   // comma = OR (either role works)
[HttpPut("{id}")]
public IActionResult Update(int id) => NoContent();
```

## Policy-based authorization (more flexible, recommended for anything beyond simple roles)

Register policies in `Program.cs`:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MinimumAge18", policy =>
        policy.RequireClaim("Age", "18", "19", "20" /* ... */));

    options.AddPolicy("MustBeBookOwner", policy =>
        policy.Requirements.Add(new BookOwnerRequirement()));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});
```

Use it:
```csharp
[Authorize(Policy = "AdminOnly")]
[HttpDelete("{id}")]
public IActionResult Delete(int id) => NoContent();
```

## Custom authorization handlers (resource-based authorization)

For rules like "a user can only edit their own book", you need custom logic that inspects the specific resource:

```csharp
public class BookOwnerRequirement : IAuthorizationRequirement { }

public class BookOwnerHandler : AuthorizationHandler<BookOwnerRequirement, Book>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BookOwnerRequirement requirement,
        Book resource)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (resource.OwnerId.ToString() == userId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
```

Register:
```csharp
builder.Services.AddScoped<IAuthorizationHandler, BookOwnerHandler>();
```

Use in a controller (imperative style, since it needs the actual resource):
```csharp
public class BooksController : ControllerBase
{
    private readonly IAuthorizationService _authService;
    public BooksController(IAuthorizationService authService) => _authService = authService;

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBookDto dto)
    {
        var book = await _repo.GetByIdAsync(id);
        if (book is null) return NotFound();

        var result = await _authService.AuthorizeAsync(User, book, "MustBeBookOwner");
        if (!result.Succeeded) return Forbid();

        // proceed with update
        return NoContent();
    }
}
```

## 401 vs 403 — know the difference

| Code | Meaning | When |
|---|---|---|
| `401 Unauthorized` | "I don't know who you are" | Missing/invalid/expired token |
| `403 Forbidden` | "I know who you are, but you can't do this" | Valid token, insufficient permissions |

`[Authorize]` returns 401 automatically when there's no valid token; your code returns `Forbid()` (403) when the user is known but not permitted.

## ASP.NET Core Identity (mention, not required for this course)

For projects needing full user management (registration, password reset, email confirmation, external logins like Google/Facebook), **ASP.NET Core Identity** provides all of this out of the box and integrates with JWT bearer auth. It's worth learning once you're comfortable with the manual JWT flow above — search "ASP.NET Core Identity API endpoints" for the current built-in approach.

---
**Next:** `03-Global-Error-Handling-Logging.md`
