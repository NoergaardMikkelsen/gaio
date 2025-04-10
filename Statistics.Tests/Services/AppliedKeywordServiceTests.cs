using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Services.Keywords;
using Statistics.Tests.Service.Benchmark;

namespace Statistics.Tests.Service;

[TestFixture]
public class AppliedKeywordServiceTests
{
    private IAppliedKeywordService _service;

    [SetUp]
    public void SetUp()
    {
        _service = new AppliedKeywordService();
    }

    private static IEnumerable<IResponse> CreateResponses(
        string matchingText, DateTime? matchingCreatedDateTime = null, DateTime? nonMatchingCreatedDateTime = null)
    {
        var responses = Enum.GetValues<ArtificialIntelligenceType>().Select(aiType => new Response
        {
            Text = matchingText,
            Ai = new ArtificialIntelligence {AiType = aiType},
            CreatedDateTime = matchingCreatedDateTime ?? DateTime.UtcNow
        }).ToList();

        responses.Add(new Response
        {
            Text = "non-matching response",
            Ai = new ArtificialIntelligence {AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB},
            CreatedDateTime = nonMatchingCreatedDateTime ?? DateTime.UtcNow.AddDays(-2)
        });

        return responses;
    }

    [Test]
    public async Task GenerateAppliedKeywordUsingRegex_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = true};
        var responses = CreateResponses("test response").ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword}, responses);

        // Assert
        foreach (var aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType)
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordUsingContains_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = false};
        var responses = CreateResponses("test response").ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword}, responses);

        // Assert
        foreach (var aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType)
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordWithStartOnlyUsingRegex_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = true, StartSearch = DateTime.UtcNow.AddDays(-1)};
        var responses = CreateResponses("test response", matchingCreatedDateTime: DateTime.UtcNow).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword}, responses);

        // Assert
        foreach (var aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType)
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordWithStartOnlyUsingContains_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = false, StartSearch = DateTime.UtcNow.AddDays(-1)};
        var responses = CreateResponses("test response", matchingCreatedDateTime: DateTime.UtcNow).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword}, responses);

        // Assert
        foreach (var aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType)
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordWithEndOnlyUsingRegex_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = true, EndSearch = DateTime.UtcNow.AddDays(-1)};
        var responses =
            CreateResponses("test response", matchingCreatedDateTime: DateTime.UtcNow.AddDays(-2)).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword}, responses);

        // Assert
        foreach (var aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType)
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordWithEndOnlyUsingContains_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword {Text = "test", UseRegex = false, EndSearch = DateTime.UtcNow.AddDays(-1)};
        var responses =
            CreateResponses("test response", matchingCreatedDateTime: DateTime.UtcNow.AddDays(-2)).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword}, responses);

        // Assert
        foreach (var aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType)
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordForTimeRangeUsingRegex_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword
        {
            Text = "test",
            UseRegex = true,
            StartSearch = DateTime.UtcNow.AddDays(-2),
            EndSearch = DateTime.UtcNow.AddDays(-1)
        };
        var responses = CreateResponses("test response",
            matchingCreatedDateTime: DateTime.UtcNow.AddDays(-2), nonMatchingCreatedDateTime: DateTime.UtcNow.AddDays(-1)).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword}, responses);

        // Assert
        foreach (var aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType)
            });
        }
    }

    [Test]
    public async Task GenerateAppliedKeywordForTimeRangeUsingContains_ShouldReturnCorrectResult()
    {
        // Arrange
        var keyword = new Keyword
        {
            Text = "test",
            UseRegex = false,
            StartSearch = DateTime.UtcNow.AddDays(-2),
            EndSearch = DateTime.UtcNow.AddDays(-1)
        };
        var responses = CreateResponses("test response",
            matchingCreatedDateTime: DateTime.UtcNow.AddDays(-2), nonMatchingCreatedDateTime: DateTime.UtcNow.AddDays(-1)).ToList();

        // Act
        var result = await _service.CalculateAppliedKeywords(new[] {keyword}, responses);

        // Assert
        foreach (var aiType in Enum.GetValues<ArtificialIntelligenceType>())
        {
            result.Should().ContainSingle(x => x.AiType == aiType).Which.Should().BeEquivalentTo(new
            {
                Text = "test",
                AiType = aiType,
                MatchingResponsesCount = 1,
                TotalResponsesCount = responses.Count(x => x.Ai.AiType == aiType)
            });
        }
    }

    [Test]
    public void BenchmarkPerformance()
    {
        ManualConfig config = ManualConfig.Create(DefaultConfig.Instance)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator).AddLogger(ConsoleLogger.Default)
            .AddExporter(MarkdownExporter.GitHub).AddExporter(HtmlExporter.Default).AddExporter(JsonExporter.Default)
            .AddValidator(JitOptimizationsValidator.DontFailOnError).AddFilter(new ExcludeAssembliesFilter("Uno"));

        Summary? summary = BenchmarkRunner.Run<AppliedKeywordServiceBenchmarks>(config);
    }

    public class ExcludeAssembliesFilter : IFilter
    {
        private readonly string[] _excludedAssemblies;

        public ExcludeAssembliesFilter(params string[] excludedAssemblies)
        {
            _excludedAssemblies = excludedAssemblies;
        }

        public bool Predicate(BenchmarkCase benchmarkCase)
        {
            foreach (string assembly in _excludedAssemblies)
            {
                if (benchmarkCase.Descriptor.Type.Assembly.FullName.Contains(assembly))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
