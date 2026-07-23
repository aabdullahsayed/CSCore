# 2. Authorization: Roles, Policies, and Claims

## Role-based authorization

```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id) { ... }

[Authorize(Roles = "Admin,Editor")]  // either role
[HttpPut("{id:int}")]
public async Task<IActionResult> Update(int id, UpdateBookDto dto) { ... }
```

Roles come from a `ClaimTypes.Role` claim embedded in the JWT (see previous file).

## Policy-based authorization (more flexible)

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MinimumAge18", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "DateOfBirth") &&
            DateTime.Parse(context.User.FindFirst("DateOfBirth")!.Value).AddYears(18) <= DateTime.Now));

    options.AddPolicy("CanManageBooks", policy =>
        policy.RequireRole("Admin", "Editor")
              .RequireClaim("Department", "Publishing"));
});
```

```csharp
[Authorize(Policy = "CanManageBooks")]
[HttpPost]
public async Task<IActionResult> Create(CreateBookDto dto) { ... }
```

## Custom authorization handlers

For complex, reusable logic:

```csharp
public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }
    public MinimumAgeRequirement(int minimumAge) => MinimumAge = minimumAge;
}

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        var dobClaim = context.User.FindFirst("DateOfBirth");
        if (dobClaim != null && DateTime.Parse(dobClaim.Value).AddYears(requirement.MinimumAge) <= DateTime.Now)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
```

```csharp
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddAuthorization(options =>
    options.AddPolicy("MinimumAge18", policy => policy.Requirements.Add(new MinimumAgeRequirement(18))));
```

## Resource-based authorization

When the decision depends on the specific resource being accessed (e.g., "can this user edit *this* book?"):

```csharp
public class BookOwnerHandler : AuthorizationHandler<SameAuthorRequirement, Book>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, SameAuthorRequirement requirement, Book resource)
    {
        var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (resource.OwnerId.ToString() == userId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
```

```csharp
var result = await _authorizationService.AuthorizeAsync(User, book, "SameAuthorPolicy");
if (!result.Succeeded) return Forbid();
```

## `[AllowAnonymous]`

Explicitly opt an endpoint out of authorization, useful when a controller-level `[Authorize]` applies globally:

```csharp
[AllowAnonymous]
[HttpGet]
public async Task<IActionResult> GetAll() { ... }  // public even if controller requires auth elsewhere
```

## Practice task

Add an `Admin`-only policy requiring both the `Admin` role and a `Department=Publishing` claim, then apply it to the `DELETE /api/books/{id}` endpoint.

**Next → `03-api-versioning.md`**
