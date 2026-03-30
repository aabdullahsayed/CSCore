## Attribute Routing

Routes are defined directly on `Controllers` or `Action methods` using attributes. This is the preferred method for REST APIs because it makes the URL structure explicit and easy to read.
How it works: You use` [Route("path")]` or HTTP verb attributes like `[HttpGet("id")]`.

## Convention Based Routing

This defines a global template (convention) that all URLs must follow. Instead of labeling every action, you create a pattern that the engine uses to "guess" which controller to call.

`The Pattern: {controller}/{action}/{id?}`

Example: A request to /Products/Details/5 automatically looks for ProductsController, the Details method, and passes 5 as the ID.

Best For: Traditional MVC web apps with views.


## Endpoint Routing

Minimal APIs use extension methods (like MapGet, MapPost, etc.) to map a URI pattern directly to a delegate (an anonymous function or a method).




## All three routing types (Minimal API, Controller-based, and Convention-based)

```c#
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(); // Required for Controllers
var app = builder.Build();

// 1. Minimal API (Endpoint Routing)
// Defined directly in the pipeline for high performance.
app.MapGet("/minimal", () => "Hello from Minimal API!");

// 2. Attribute Routing (Maps to Controllers)
// This looks for [Route] and [HttpGet] attributes on your classes.
app.MapControllers();

// 3. Convention-Based Routing
// A global pattern used typically for MVC Views.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```