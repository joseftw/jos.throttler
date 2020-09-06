using BenchmarkDotNet.Running;

namespace JOS.Throttler.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ThrottlerBenchmark>();
        }
    }
}
