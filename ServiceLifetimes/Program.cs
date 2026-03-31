var builder = WebApplication.CreateBuilder(args);

// Registering the lifetimes
builder.Services.AddTransient<ITransientService, ExampleService>();
builder.Services.AddScoped<IScopedService, ExampleService>();
builder.Services.AddSingleton<ISingletonService, ExampleService>();

var app = builder.Build();

app.MapGet("/", (
    ITransientService t1, ITransientService t2, 
    IScopedService s1, IScopedService s2, 
    ISingletonService singleton) => 
{
    // t1 and t2 will have DIFFERENT IDs (new every time)
    // s1 and s2 will have the SAME ID (shared for this specific request)
    // singleton will have the SAME ID (shared for all users, forever)
    
    return new {
        Transient = new { id1 = t1.Id, id2 = t2.Id },
        Scoped = new { id1 = s1.Id, id2 = s2.Id },
        Singleton = singleton.Id
    };
});

app.Run();

// A simple class to track unique instances
public class ExampleService : ITransientService, IScopedService, ISingletonService 
{
    public Guid Id { get; } = Guid.NewGuid();
}

public interface ITransientService { Guid Id { get; } }
public interface IScopedService { Guid Id { get; } }
public interface ISingletonService { Guid Id { get; } }