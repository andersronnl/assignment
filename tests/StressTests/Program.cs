using System;
using System.Threading.Tasks;

namespace StressTests;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Running insurance service stress test...");
        await new InsuranceStressTest().Run();
    }
}
