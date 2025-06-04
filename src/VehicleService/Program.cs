using VehicleServiceApp.Services;
using VehicleServiceApp.Models;
using VehicleServiceApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // .NET 9 uses AddOpenApi instead of AddSwaggerGen

// Register VehicleService
builder.Services.AddScoped<IVehicleService, VehicleService>();

// Register exception middleware
builder.Services.AddTransient<ExceptionMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // .NET 9 uses MapOpenApi instead of UseSwagger/UseSwaggerUI
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Use custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
