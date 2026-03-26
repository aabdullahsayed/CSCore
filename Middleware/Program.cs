var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Use(async (HttpContext, next) =>
    {
        Console.WriteLine("Goes through middleware 1");
        await next();
        Console.WriteLine("Coming back through middleware 1");
    }
);


app.Use(async (HttpContext, next) =>
    {
        Console.WriteLine("Goes through Middleware 2");
        await next();
        Console.WriteLine("Coming back through middleware 2");
    }
    );

app.Map("/", ()=> "Hello world");



app.Run();
