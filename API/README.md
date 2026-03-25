1. `API.csproj` The heart of project configuration.
2. `Program.cs` Entry Point
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

`CreateBuilder()` → sets up default hosting (Kestrel server, config, logging)
`Build()` → creates the app
`Run()` → starts listening for requests

