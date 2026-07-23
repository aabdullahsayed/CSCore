# 6. Security Hardening

## OWASP API Security Top 10 — the checklist

A production API should be evaluated against the OWASP API Security Top 10. Key items and how ASP.NET Core addresses them:

### 1. Broken Object Level Authorization
Always verify the authenticated user actually owns/can access the specific resource requested — don't just check they're logged in.

```csharp
[HttpGet("{id:int}")]
public async Task<IActionResult> GetById(int id)
{
    var book = await _repository.GetByIdAsync(id);
    if (book is null) return NotFound();

    var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    if (book.OwnerId.ToString() != userId) return Forbid();

    return Ok(book);
}
```

### 2. Broken Authentication
- Enforce strong password policies and hash with `BCrypt` or `PasswordHasher<T>` (never plain text or weak hashing like MD5/SHA1).
- Use short-lived JWTs with refresh tokens.
- Rate-limit login endpoints to prevent brute force.

### 3. Excessive Data Exposure
Always return DTOs, never raw entities — prevents accidentally leaking internal fields (password hashes, internal flags).

### 4. Lack of Resources & Rate Limiting
Covered in `05-performance-optimization.md` — always cap payload sizes and request rates.

```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});
```

### 5. Mass Assignment
Never bind request bodies directly to EF Core entities — use DTOs so clients can't set fields like `IsAdmin` or `Id` they shouldn't control (covered in `02-intermediate/03-model-validation.md`).

### 6. Security Misconfiguration
- Disable detailed error pages/stack traces outside Development.
- Set secure HTTP headers.
- Keep the framework and NuGet packages patched (`dotnet list package --vulnerable`).

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    await next(context);
});

app.UseHsts();
```

### 7. Injection
EF Core's LINQ queries are parameterized by default — safe from SQL injection as long as you don't build raw SQL by string concatenation.

```csharp
// ❌ Vulnerable
var sql = $"SELECT * FROM Books WHERE Title = '{title}'";

// ✅ Safe — parameterized
await _context.Books.FromSqlInterpolated($"SELECT * FROM Books WHERE Title = {title}").ToListAsync();
// or simply use LINQ, which is always parameterized:
await _context.Books.Where(b => b.Title == title).ToListAsync();
```

### 8. Improper Assets Management
Keep track of every API version and endpoint exposed; retire and remove deprecated/unused versions rather than leaving them silently running (see `03-advanced/03-api-versioning.md`).

### 9. Insufficient Logging & Monitoring
Log authentication failures, authorization failures, and unusual patterns — but **never log secrets, passwords, or full tokens**.

### 10. Server-Side Request Forgery (SSRF)
If your API fetches URLs supplied by users (e.g., an avatar image URL), validate and restrict destinations — don't let user input make your server call arbitrary internal addresses.

## HTTPS and HSTS

```csharp
app.UseHttpsRedirection();
app.UseHsts();  // Production only — instructs browsers to always use HTTPS for this domain
```

## Secrets management

- **Development**: `dotnet user-secrets` (never commit secrets to source control).
- **Production**: Azure Key Vault, AWS Secrets Manager, HashiCorp Vault — inject at startup, never hardcode.

## Input sanitization

ASP.NET Core's model binding + validation attributes (covered in `02-intermediate/03-model-validation.md`) form the first line of defense. For any content rendered elsewhere as HTML (rare in pure APIs, common if you also serve a front-end), always encode output to prevent XSS.

## Dependency scanning

```bash
dotnet list package --vulnerable --include-transitive
```

Run this regularly (and in CI) to catch known-vulnerable NuGet packages before they ship.

## Practice task

Run through the OWASP checklist against your Books API: confirm DTOs are used everywhere, add security headers middleware, verify resource-level authorization on the `GetById`/`Update`/`Delete` endpoints, and run `dotnet list package --vulnerable`.

**Next → `07-capstone-project.md`**
