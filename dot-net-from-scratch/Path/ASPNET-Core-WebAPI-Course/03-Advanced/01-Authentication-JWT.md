# 1. Authentication with JWT

## Authentication vs Authorization

- **Authentication** = "Who are you?" (verifying identity, e.g., login)
- **Authorization** = "What are you allowed to do?" (permissions, roles)

This file covers authentication; the next covers authorization.

## What is a JWT?

A **JSON Web Token** is a signed, self-contained token that encodes claims about a user (id, roles, expiry) as a compact string. The server verifies the signature — it doesn't need to look up a session in a database on every request.

```
header.payload.signature
```

Flow:
```
1. Client POSTs credentials to /api/auth/login
2. Server verifies credentials, generates a signed JWT
3. Client stores the JWT (e.g., in memory or secure storage) and
   sends it in the Authorization header on every subsequent request:
   Authorization: Bearer <token>
4. Server validates the token's signature & expiry on each request
```

## Install packages

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

## Configuration (appsettings.json)

```json
{
  "Jwt": {
    "Key": "THIS_IS_A_DEV_ONLY_SECRET_REPLACE_IN_PRODUCTION_MIN_32_CHARS",
    "Issuer": "BookStore.Api",
    "Audience": "BookStore.Client",
    "ExpiryMinutes": 60
  }
}
```

> ⚠️ Never commit real secrets to source control. Use **User Secrets** in development (`dotnet user-secrets set "Jwt:Key" "..."`) and environment variables / a secret manager (Azure Key Vault, AWS Secrets Manager) in production.

## Registering JWT authentication in Program.cs

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();   // must come before UseAuthorization
app.UseAuthorization();
app.MapControllers();
```

## Generating a token on login

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class TokenService
{
    private readonly IConfiguration _config;
    public TokenService(IConfiguration config) => _config = config;

    public string GenerateToken(string userId, string email, IEnumerable<string> roles)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

## The login endpoint

```csharp
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly IUserRepository _users;

    public AuthController(TokenService tokenService, IUserRepository users)
    {
        _tokenService = tokenService;
        _users = users;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email, user.Roles);
        return Ok(new { token });
    }
}
```

> Passwords must **never** be stored in plain text. Use a hashing library like `BCrypt.Net-Next` (`dotnet add package BCrypt.Net-Next`) — hash on registration, verify on login.

## Protecting endpoints

```csharp
[Authorize]                          // requires any valid, authenticated JWT
[HttpGet("me")]
public IActionResult Me()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Ok(new { userId });
}
```

## Testing with curl

```bash
# Login
curl -X POST https://localhost:7031/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@test.com","password":"P@ssw0rd"}'

# Use the returned token
curl https://localhost:7031/api/auth/me \
  -H "Authorization: Bearer eyJhbGciOi..."
```

## Refresh tokens (brief note)

Access tokens should be short-lived (15–60 min). For a seamless UX without forcing re-login, pair them with a longer-lived **refresh token** stored securely (httpOnly cookie or database-backed), exchanged for a new access token when it expires. This is a common production pattern but adds complexity — implement it once the basic JWT flow above is solid.

---
**Next:** `02-Authorization-Roles-Policies.md`
