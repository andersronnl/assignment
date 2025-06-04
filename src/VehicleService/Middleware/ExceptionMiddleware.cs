using VehicleServiceApp.Models;

namespace VehicleServiceApp.Middleware;

public class ExceptionMiddleware(RequestDelegate Next, ILogger<ExceptionMiddleware> Logger)
{
    public async Task InvokeAsync(HttpContext Context)
    {
        try
        {
            await Next(Context);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "An unhandled exception occurred.");
            await HandleExceptionAsync(Context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext Context, Exception Exception)
    {
        Context.Response.ContentType = "application/json";
        var response = new ApiResponse<object>
        {
            Success = false,
            ErrorMessage = "An unexpected error occurred"
        };
        
        Context.Response.StatusCode = Exception switch
        {
            HttpRequestException => 503,
            ArgumentException => 400,
            _ => 500
        };
        
        await Context.Response.WriteAsJsonAsync(response);
    }
}