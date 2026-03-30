var builder = WebApplication.CreateBuilder(args);

// This tells the framework: "Please scan my project, find any classes 
// with [ApiController], and register them so they can be created."
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});



var app = builder.Build();

//This tells the routing engine: "Please look at all the [Route] attributes 
// on my controllers and draw the map so Kestrel knows where to send traffic.
app.MapControllers();

app.UseCors("AllowReact");
    
app.Run();
