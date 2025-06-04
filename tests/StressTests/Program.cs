namespace StressTests;

internal static class Program // ReSharper disable once ClassNeverInstantiated.Global
{
    private static async Task Main(string[] _) // ReSharper disable once UnusedParameter.Local
    {
        Console.WriteLine("Running insurance service stress test...");
        await InsuranceStressTest.Run();
    }
}
