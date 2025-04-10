using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Statistics.Tests.Benchmark.Core;

namespace Statistics.Tests.Benchmark;

[TestFixture]
public class BenchmarkTests
{
    [Test]
    public void BenchmarkAppliedKeywordService()
    {
        var config = new BenchmarkConfig();

        Summary? summary = BenchmarkRunner.Run<AppliedKeywordServiceBenchmarks>(config);

        // Check if the benchmark execution contains any errors
        if (summary == null || summary.HasCriticalValidationErrors)
        {
            Assert.Fail("Benchmark failed due to critical validation errors.");
        }
    }
}
