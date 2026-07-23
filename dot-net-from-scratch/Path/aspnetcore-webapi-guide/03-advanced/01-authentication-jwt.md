# 1. Authentication with JWT

## Authentication vs. Authorization

- **Authentication** — *who are you?* (verifying identity, e.g., via login credentials)
- **Authorization** — *what are you allowed to do?* (covered in the next file)

## Why JWT (JSON Web Tokens)?

JWTs are self-contained, signed tokens that let APIs be **stateless** — no server-side session storage needed. The client sends the token in the `Authorization` header on every request; the server verifies its signature and reads claims from it directly.

```
Header.Payload.Signature
eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjMiLCJuYW1lIjoiSm9obiJ9.SflKxwRJSMeKKF2QT4fwpMe...
```

## Install packages

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

## Configure JWT authentication

```csharp
// appsettings.json
// "Jwt": { "Key": "a-very-long-random-secret-key-min-32-chars", "Issuer": "MyApi", "Audience": "MyApiUsers", "ExpiryMinutes": 60 }

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
```

```csharp
// Pipeline — order matters: Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();
```

## Generating a token on login

```csharp
public class TokenService
{
    private readonly IConfiguration _config;
    public TokenService(IConfiguration config) => _config = config;

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiryMinutes"]!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly IUserService _userService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userService.ValidateCredentialsAsync(dto.Email, dto.Password);
        if (user is null) return Unauthorized("Invalid credentials.");

        var token = _tokenService.GenerateToken(user);
        return Ok(new { token });
    }
}
```

⚠️ **Never store plain-text passwords.** Hash with `BCrypt.Net` or ASP.NET Core Identity's `PasswordHasher<T>`:

```bash
dotnet add package BCrypt.Net-Next
```

```csharp
string hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
bool valid = BCrypt.Net.BCrypt.Verify(plainPassword, hash);
```

## Protecting endpoints

```csharp
[Authorize]
[HttpGet("profile")]
public IActionResult GetProfile()
{
    var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    return Ok(new { userId });
}
```

## Refresh tokens (brief overview)

Access tokens should be short-lived (15–60 min). Pair them with a longer-lived **refresh token** (stored securely, often in an httpOnly cookie or a database) to issue new access tokens without forcing re-login. Full implementation involves a `/auth/refresh` endpoint that validates the refresh token and issues a new access token.

## Practice task

Add `User` registration/login endpoints with hashed passwords, issue JWTs on login, and protect the Books `POST`/`PUT`/`DELETE` endpoints with `[Authorize]`.

**Next → `02-authorization-policies.md`**
