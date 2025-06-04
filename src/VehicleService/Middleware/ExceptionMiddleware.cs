using System.Net;
using VehicleServiceApp.Models;

namespace VehicleServiceApp.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new ApiResponse<object>
        {
            Success = false,
            ErrorMessage = "An unexpected error occurred"
        };
        
        context.Response.StatusCode = exception switch
        {
            HttpRequestException => 503,
            ArgumentException => 400,
            _ => 500
        };
        
        await context.Response.WriteAsJsonAsync(response);
    }
}