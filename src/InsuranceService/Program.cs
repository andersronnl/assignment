using InsuranceServiceApp.Services;
using InsuranceServiceApp.Models;
using InsuranceServiceApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // .NET 9 uses AddOpenApi instead of AddSwaggerGen

// Register HttpClientFactory with typed client
builder.Services.AddHttpClient<IVehicleApiClient, VehicleApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:VehicleService"] ?? throw new InvalidOperationException("VehicleService URL is not configured"));
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<IInsuranceService, InsuranceServiceApp.Services.InsuranceService>();

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

app.Run();
