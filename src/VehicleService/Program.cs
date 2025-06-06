using VehicleServiceApp.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // .NET 9 uses AddOpenApi instead of AddSwaggerGen
builder.Services.AddProblemDetails(); // Add ProblemDetails for standardized error responses

// Register VehicleService
builder.Services.AddScoped<IVehicleService, VehicleService>();

var app = builder.Build();

// Use built-in exception handler instead of custom middleware
app.UseExceptionHandler(ExceptionHandlerApp =>
{
    ExceptionHandlerApp.Run(async Context =>
    {
        Context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        Context.Response.ContentType = "application/json";
        
        var exceptionHandlerFeature = Context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            var exception = exceptionHandlerFeature.Error;
            var problemDetails = new ProblemDetails
            {
                Status = Context.Response.StatusCode,
                Title = "An unexpected error occurred",
                Detail = exception.Message
            };
            
            await Context.Response.WriteAsJsonAsync(problemDetails);
        }
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // .NET 9 uses MapOpenApi instead of UseSwagger/UseSwaggerUI
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
