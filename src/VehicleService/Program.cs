using VehicleServiceApp.Services;
using VehicleServiceApp.Middleware;

var builder = WebApplication.CreateBuilder(args); // ReSharper disable once UnusedParameter.Local

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // .NET 9 uses AddOpenApi instead of AddSwaggerGen

// Register VehicleService
builder.Services.AddScoped<IVehicleService, VehicleService>();

var app = builder.Build();

// Register exception middleware first to catch errors
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // .NET 9 uses MapOpenApi instead of UseSwagger/UseSwaggerUI
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
