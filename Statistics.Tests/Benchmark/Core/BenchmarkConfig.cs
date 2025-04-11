using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace Statistics.Tests.Benchmark.Core;

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        Add(DefaultConfig.Instance);

        AddLogger(ConsoleLogger.Default);
        //AddExporter(MarkdownExporter.GitHub); // Already Present
        //AddExporter(HtmlExporter.Default); // Already Present
        AddExporter(JsonExporter.Default);
        AddValidator(JitOptimizationsValidator.DontFailOnError);
        WithOptions(ConfigOptions.DisableOptimizationsValidator);
        //WithOptions(ConfigOptions.LogBuildOutput);

        AddFilter(new ExcludeAssembliesFilter("Uno"));
    }

    private class ExcludeAssembliesFilter : IFilter
    {
        private readonly string[] _excludedAssemblies;

        public ExcludeAssembliesFilter(params string[] excludedAssemblies)
        {
            _excludedAssemblies = excludedAssemblies;
        }

        public bool Predicate(BenchmarkCase benchmarkCase)
        {
            foreach (string? assembly in _excludedAssemblies)
            {
                if (!benchmarkCase.Descriptor.Type.Assembly.FullName.Contains(assembly))
                {
                    continue;
                }

                ConsoleLogger.Default.WriteLine(LogKind.Info,
                    $"Assembly '{benchmarkCase.Descriptor.Type.Assembly.FullName}' is being ignored, due to excluding '{assembly}'.");
                return false;
            }

            return true;
        }
    }
}
