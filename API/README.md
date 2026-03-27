1. `API.csproj` The heart of project configuration.
   **Defines:**
- Target framework (net8.0, etc.)
- Dependencies (NuGet packages)
- Build settings

2. `Program.cs` Entry Point
- App starts
- Services are registered
- Middleware pipeline is built
- Routes are mapped

3. `appsettings.json` Configuration (Stores global settings)
4. `appsettings.development.json` Overrides `appsettings.json` in development
5. **Properties** folder contains launch settings, it defines port, enviornment and how app runs locally
6. `APP.http` Testing file (Build in API testing tool)
7. This structure is bare minimum

`Program.cs`
```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run();
```

`CreateBuilder()` → sets up default hosting (Kestrel server, config, logging) <br>
`Build()` → creates the app <br>
`Run()` → starts listening for requests

