using BenchmarkDotNet.Attributes;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Abstraction.Interfaces.Services;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Services.Keywords;

namespace Statistics.Tests.Benchmark;

[MemoryDiagnoser]
public class AppliedKeywordServiceBenchmarks
{
    private readonly IAppliedKeywordService _service;
    private readonly IEnumerable<IKeyword> _keywords;
    private readonly IEnumerable<IResponse> _responses;

    public AppliedKeywordServiceBenchmarks()
    {
        _service = new AppliedKeywordService();
        _keywords = new List<IKeyword>
        {
            new Keyword
            {
                Text = "test", UseRegex = true, StartSearch = DateTime.UtcNow.AddDays(-1),
                EndSearch = DateTime.UtcNow.AddDays(-1),
            },
            new Keyword
            {
                Text = "test", UseRegex = false, StartSearch = DateTime.UtcNow.AddDays(-1),
                EndSearch = DateTime.UtcNow.AddDays(-1),
            },
        };
        _responses = new List<IResponse>
        {
            new Response
            {
                Text = "test response",
                Ai = new ArtificialIntelligence() {AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB,},
                CreatedDateTime = DateTime.UtcNow,
            },
            new Response
            {
                Text = "another response",
                Ai = new ArtificialIntelligence() {AiType = ArtificialIntelligenceType.OPEN_AI_NO_WEB,},
                CreatedDateTime = DateTime.UtcNow.AddDays(-2),
            },
        };
    }

    [Benchmark]
    public async Task GenerateAppliedKeywordUsingRegex()
    {
        await _service.CalculateAppliedKeywords(_keywords.Where(k => k.UseRegex), _responses);
    }

    [Benchmark]
    public async Task GenerateAppliedKeywordUsingContains()
    {
        await _service.CalculateAppliedKeywords(_keywords.Where(k => !k.UseRegex), _responses);
    }

    [Benchmark]
    public async Task GenerateAppliedKeywordWithStartOnlyUsingRegex()
    {
        await _service.CalculateAppliedKeywords(
            _keywords.Where(k => k.UseRegex && k.StartSearch != null && k.EndSearch == null), _responses);
    }

    [Benchmark]
    public async Task GenerateAppliedKeywordWithStartOnlyUsingContains()
    {
        await _service.CalculateAppliedKeywords(
            _keywords.Where(k => !k.UseRegex && k.StartSearch != null && k.EndSearch == null), _responses);
    }

    [Benchmark]
    public async Task GenerateAppliedKeywordWithEndOnlyUsingRegex()
    {
        await _service.CalculateAppliedKeywords(
            _keywords.Where(k => k.UseRegex && k.StartSearch == null && k.EndSearch != null), _responses);
    }

    [Benchmark]
    public async Task GenerateAppliedKeywordWithEndOnlyUsingContains()
    {
        await _service.CalculateAppliedKeywords(
            _keywords.Where(k => !k.UseRegex && k.StartSearch == null && k.EndSearch != null), _responses);
    }

    [Benchmark]
    public async Task GenerateAppliedKeywordForTimeRangeUsingRegex()
    {
        await _service.CalculateAppliedKeywords(
            _keywords.Where(k => k.UseRegex && k.StartSearch != null && k.EndSearch != null), _responses);
    }

    [Benchmark]
    public async Task GenerateAppliedKeywordForTimeRangeUsingContains()
    {
        await _service.CalculateAppliedKeywords(
            _keywords.Where(k => !k.UseRegex && k.StartSearch != null && k.EndSearch != null), _responses);
    }
}
