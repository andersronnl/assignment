using System.Diagnostics;

namespace StressTests;

public static class InsuranceStressTest
{
    private static readonly string _baseUrl = Environment.GetEnvironmentVariable("INSURANCE_SERVICE_BASE_URL") ?? "http://localhost:5083"; // Default value
    private const int RequestCount = 1000;
    private const int MaxParallelRequests = 100;

    public static async Task Run()
    {
        Console.WriteLine($"Starting stress test with {RequestCount} requests...");
        var stopwatch = Stopwatch.StartNew();
        var completed = 0;
        var errors = 0;
        
        using (var httpClient = new HttpClient()) // Local HttpClient
        {
            var tasks = new Task[RequestCount];
            for (var i = 0; i < RequestCount; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    try
                    {
                        var response = await httpClient.GetAsync($"{_baseUrl}/api/insurances/12345");
                        response.EnsureSuccessStatusCode();
                        Interlocked.Increment(ref completed);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during request: {ex.Message}"); // Log exception
                        Interlocked.Increment(ref errors);
                    }
                });

                // Throttle to avoid overwhelming the system
                if (i % MaxParallelRequests == 0)
                {
                    await Task.Delay(100);
                }
            }
            await Task.WhenAll(tasks);
        }
        stopwatch.Stop();

        Console.WriteLine($"Stress test completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        Console.WriteLine($"Successful requests: {completed}/{RequestCount}");
        Console.WriteLine($"Failed requests: {errors}/{RequestCount}");
        Console.WriteLine($"Requests/sec: {RequestCount / stopwatch.Elapsed.TotalSeconds:F2}");
    }
}