using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace StressTests;

public class InsuranceStressTest
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string BaseUrl = "http://localhost:5001";
    private const int RequestCount = 1000;
    private const int MaxParallelRequests = 100;

    public async Task Run()
    {
        Console.WriteLine($"Starting stress test with {RequestCount} requests...");
        var stopwatch = Stopwatch.StartNew();
        var completed = 0;
        var errors = 0;

        var tasks = new Task[RequestCount];
        for (int i = 0; i < RequestCount; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{BaseUrl}/api/insurances/12345");
                    response.EnsureSuccessStatusCode();
                    Interlocked.Increment(ref completed);
                }
                catch
                {
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
        stopwatch.Stop();

        Console.WriteLine($"Stress test completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        Console.WriteLine($"Successful requests: {completed}/{RequestCount}");
        Console.WriteLine($"Failed requests: {errors}/{RequestCount}");
        Console.WriteLine($"Requests/sec: {RequestCount / stopwatch.Elapsed.TotalSeconds:F2}");
    }
}